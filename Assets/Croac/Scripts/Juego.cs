using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Croac
{
    /// <summary>
    /// La partida: crea las filas de nenúfares, hace saltar a la rana con cada toque, cuenta los puntos
    /// y guarda el récord al caer al agua.
    /// </summary>
    public class Juego : MonoBehaviour, IJugador
    {
        [SerializeField] Nivel nivelPorDefecto;
        [SerializeField] Nenufar prefabNenufar;
        [SerializeField] Rana rana;
        [SerializeField] Camera camara;
        [SerializeField] SpriteRenderer fondo;
        [SerializeField] Marcador marcador;
        [SerializeField] GameObject panelFin;
        [SerializeField] TMP_Text textoFin;
        [SerializeField] AudioSource altavoz;
        [SerializeField] AudioClip sonidoSalto;
        [SerializeField] AudioClip sonidoMosca;
        [Tooltip("Distancia horizontal máxima entre la rana y el centro del nenúfar para aterrizar.")]
        [SerializeField] float radioAterrizaje = 0.65f;
        [SerializeField] int filasPorDelante = 8;
        [SerializeField] int filasPorDetras = 2;

        readonly List<Nenufar> filas = new();
        readonly Puntuacion puntuacion = new();
        Nivel nivel;
        GeneradorNenufares generador;
        InputAction accionSaltar;
        int primeraFila;
        int filaActual;

        public bool Terminada { get; private set; }
        public bool PuedeSaltar => !Terminada && !rana.EnElAire;
        public float DesvioSiguiente => Fila(filaActual + 1).transform.position.x - rana.transform.position.x;

        void OnEnable()
        {
            accionSaltar = InputSystem.actions.FindAction("Juego/Saltar", true);
            accionSaltar.performed += AlTocar;
            rana.Aterrizo += AlAterrizar;
        }

        void OnDisable()
        {
            accionSaltar.performed -= AlTocar;
            rana.Aterrizo -= AlAterrizar;
        }

        async Awaitable Start()
        {
            nivel = Seleccion.Nivel != null ? Seleccion.Nivel : nivelPorDefecto;
            generador = new GeneradorNenufares(nivel);
            CrearFila(new Fila(0f, 0f, false));
            RellenarFilas();
            rana.transform.position = Fila(0).transform.position;
            rana.transform.SetParent(Fila(0).transform, true);
            marcador.Mostrar(0);
            fondo.sprite = await nivel.fondo.LoadAssetAsync<Sprite>();
        }

        void OnDestroy()
        {
            if (nivel != null) nivel.fondo.ReleaseAsset();
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
            marcador.Mostrar(puntuacion.Puntos);
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

        void RellenarFilas()
        {
            while (primeraFila + filas.Count <= filaActual + filasPorDelante) CrearFila(generador.Siguiente());
            while (filaActual - primeraFila > filasPorDetras)
            {
                Destroy(filas[0].gameObject);
                filas.RemoveAt(0);
                primeraFila++;
            }
        }

        void CrearFila(Fila fila)
        {
            int indice = primeraFila + filas.Count;
            var nenufar = Instantiate(prefabNenufar, new Vector3(fila.X, Altura(indice), 0f), Quaternion.identity, transform);
            nenufar.name = $"Nenufar {indice}";
            nenufar.Configurar(fila, nivel.limite);
            filas.Add(nenufar);
        }

        Nenufar Fila(int indice) => filas[indice - primeraFila];

        float Altura(int fila) => fila * nivel.separacionFilas;
    }
}
