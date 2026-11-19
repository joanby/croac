using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Croac
{
    /// <summary>
    /// La partida: crea las filas de nenúfares, hace saltar a la rana con cada toque, cuenta los puntos
    /// y guarda el récord al caer al agua y cada vez que la app se va a segundo plano (<see cref="CicloVida"/>).
    /// </summary>
    public class Juego : MonoBehaviour, IJugador
    {
        [SerializeField] Nivel nivelPorDefecto;
        [SerializeField] Nenufar prefabNenufar;
        [SerializeField] Rana rana;
        [SerializeField] Camera camara;
        [SerializeField] LevelLoader cargador;
        [SerializeField] GameObject panelFin;
        [SerializeField] TMP_Text textoFin;
        [SerializeField] GameObject panelPausa;
        [SerializeField] AudioSource altavoz;
        [SerializeField] AudioClip sonidoSalto;
        [SerializeField] AudioClip sonidoMosca;
        [Tooltip("Distancia horizontal máxima entre la rana y el centro del nenúfar para aterrizar.")]
        [SerializeField] float radioAterrizaje = 0.65f;
        [SerializeField] int filasPorDelante = 8;
        [SerializeField] int filasPorDetras = 2;

        readonly List<Nenufar> filas = new();
        ObjectPool<Nenufar> charca;
        readonly Puntuacion puntuacion = new();
        CicloVida cicloVida;
        Nivel nivel;
        GeneradorNenufares generador;
        InputAction accionSaltar;
        int primeraFila;
        int filaActual;

        public int Puntos => puntuacion.Puntos;

        public int Saltos => puntuacion.Saltos;

        /// <summary>Moscas en los nenúfares que están por debajo de <paramref name="arriba"/>, sin asignar memoria.</summary>
        public int MoscasPorDebajoDe(float arriba)
        {
            int moscas = 0;
            foreach (var nenufar in filas)
                if (nenufar.TieneMosca && nenufar.transform.position.y < arriba) moscas++;
            return moscas;
        }

        public bool Terminada { get; private set; }
        public bool Pausada { get; private set; }
        public bool PuedeSaltar => !Terminada && !Pausada && !rana.EnElAire;
        public float DesvioSiguiente => Fila(filaActual + 1).transform.position.x - rana.transform.position.x;

        void Awake()
        {
            cicloVida = new CicloVida(() => Guardado.GuardarRecord(puntuacion.Puntos, Guardado.Ruta));
            charca = new ObjectPool<Nenufar>(
                () => Instantiate(prefabNenufar, transform),
                nenufar => nenufar.gameObject.SetActive(true),
                nenufar => nenufar.gameObject.SetActive(false),
                defaultCapacity: filasPorDetras + 1 + filasPorDelante);
        }

        void OnEnable()
        {
            accionSaltar = InputSystem.actions.FindAction("Juego/Saltar", true);
            accionSaltar.performed += AlTocar;
            rana.Aterrizo += AlAterrizar;
            Application.lowMemory += cicloVida.AlFaltarMemoria;
        }

        void OnDisable()
        {
            accionSaltar.performed -= AlTocar;
            rana.Aterrizo -= AlAterrizar;
            Application.lowMemory -= cicloVida.AlFaltarMemoria;
        }

        void OnDestroy() => Time.timeScale = 1f;

        void OnApplicationPause(bool pausada) => cicloVida.AlPausar(pausada);

        void OnApplicationFocus(bool conFoco) => cicloVida.AlCambiarFoco(conFoco);

        async Awaitable Start()
        {
            nivel = Seleccion.Nivel != null ? Seleccion.Nivel : nivelPorDefecto;
            generador = new GeneradorNenufares(nivel);
            LlenarCharca();
            CrearFila(new Fila(0f, 0f, false));
            RellenarFilas();
            rana.transform.position = Fila(0).transform.position;
            rana.transform.SetParent(Fila(0).transform, true);
            await cargador.Cargar(nivel);
        }

        void LateUpdate()
        {
            var posicion = camara.transform.position;
            float objetivo = rana.transform.position.y + camara.orthographicSize * 0.5f;
            if (objetivo > posicion.y) posicion.y = Mathf.Lerp(posicion.y, objetivo, 1f - Mathf.Exp(-8f * Time.deltaTime));
            camara.transform.position = posicion;
        }

        public void Saltar()
        {
            if (!PuedeSaltar) return;
            rana.Saltar(Altura(filaActual + 1));
            altavoz.PlayOneShot(sonidoSalto);
        }

        public void Reiniciar() => SceneManager.LoadScene(gameObject.scene.buildIndex);

        /// <summary>Lo llama el botón de pausa del HUD: para la partida o la reanuda.</summary>
        public void AlternarPausa()
        {
            Pausada = !Pausada;
            Time.timeScale = Pausada ? 0f : 1f;
            panelPausa.SetActive(Pausada);
        }

        void AlTocar(InputAction.CallbackContext _)
        {
            if (Terminada) Reiniciar();
            else Saltar();
        }

        void AlAterrizar()
        {
            filaActual++;
            var nenufar = Fila(filaActual);
            if (Mathf.Abs(nenufar.transform.position.x - rana.transform.position.x) > radioAterrizaje)
            {
                Terminar();
                return;
            }
            rana.transform.SetParent(nenufar.transform, true);
            puntuacion.Salto();
            if (nenufar.TieneMosca)
            {
                nenufar.ComerMosca();
                puntuacion.Mosca();
                altavoz.PlayOneShot(sonidoMosca);
            }
            RellenarFilas();
        }

        void Terminar()
        {
            Terminada = true;
            rana.Hundirse();
            var guardado = Guardado.Cargar();
            bool record = guardado.Registrar(puntuacion);
            guardado.Guardar();
            textoFin.SetText(record ? "¡Plof!\n¡Récord nuevo: {0}!\n\nToca para repetir" : "¡Plof!\n{0} puntos\n\nToca para repetir", puntuacion.Puntos);
            panelFin.SetActive(true);
        }

        /// <summary>
        /// Crea al cargar el nivel todos los nenúfares que puede haber a la vez (las filas de detrás, la de la rana y
        /// las de delante) y los deja en el pool: así ningún salto tiene que instanciar uno.
        /// </summary>
        void LlenarCharca()
        {
            var reserva = new Nenufar[filasPorDetras + 1 + filasPorDelante];
            for (int i = 0; i < reserva.Length; i++) reserva[i] = charca.Get();
            foreach (var nenufar in reserva) charca.Release(nenufar);
        }

        /// <summary>
        /// Devuelve al pool las filas que quedan atrás y luego crea las de delante: en ese orden, cada fila nueva
        /// reutiliza la que se acaba de soltar y el pool no tiene que instanciar otra.
        /// </summary>
        void RellenarFilas()
        {
            while (filaActual - primeraFila > filasPorDetras)
            {
                charca.Release(filas[0]);
                filas.RemoveAt(0);
                primeraFila++;
            }
            while (primeraFila + filas.Count <= filaActual + filasPorDelante) CrearFila(generador.Siguiente());
        }

        void CrearFila(Fila fila)
        {
            int indice = primeraFila + filas.Count;
            var nenufar = charca.Get();
            nenufar.Colocar(new Vector3(fila.X, Altura(indice), 0f));
            nenufar.Configurar(fila, nivel.limite);
            filas.Add(nenufar);
        }

        Nenufar Fila(int indice) => filas[indice - primeraFila];

        float Altura(int fila) => fila * nivel.separacionFilas;
    }
}
