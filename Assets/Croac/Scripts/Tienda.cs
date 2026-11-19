using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Croac
{
    /// <summary>El panel de la tienda del menú: moscas, quitar anuncios y restaurar compras, con los precios de la tienda.</summary>
    public class Tienda : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] TMP_Text textoMoscas;
        [SerializeField] TMP_Text textoComprarMoscas;
        [SerializeField] TMP_Text textoSinAnuncios;
        [SerializeField] Button botonSinAnuncios;
        [SerializeField] TMP_Text textoAviso;

        IStore tienda;

        void OnEnable()
        {
            tienda = Servicios.Tienda;
            tienda.Preparada += Refrescar;
            tienda.Entregar += AlEntregar;
            tienda.Fallo += AlFallar;
            Refrescar();
        }

        void OnDisable()
        {
            tienda.Preparada -= Refrescar;
            tienda.Entregar -= AlEntregar;
            tienda.Fallo -= AlFallar;
        }

        /// <summary>Lo llama el botón «Tienda» del menú.</summary>
        public void Abrir(bool abrir)
        {
            panel.SetActive(abrir);
            textoAviso.SetText("");
        }

        public void ComprarMoscas() => tienda.Comprar(Catalogo.Moscas);

        public void QuitarAnuncios() => tienda.Comprar(Catalogo.SinAnuncios);

        public void Restaurar() => tienda.Restaurar();

        void AlEntregar(string producto, string pedido)
        {
            textoAviso.SetText(producto == Catalogo.Moscas ? "¡+50 moscas!" : "Anuncios quitados");
            Refrescar();
        }

        void AlFallar(string motivo) => textoAviso.SetText($"No se ha podido: {motivo}");

        void Refrescar()
        {
            var guardado = Guardado.Cargar();
            textoMoscas.SetText("Moscas: {0}", guardado.moscas);
            textoComprarMoscas.SetText($"50 moscas · {tienda.Precio(Catalogo.Moscas)}");
            textoSinAnuncios.SetText(guardado.sinAnuncios ? "Sin anuncios (comprado)" : $"Quitar anuncios · {tienda.Precio(Catalogo.SinAnuncios)}");
            botonSinAnuncios.interactable = !guardado.sinAnuncios;
        }
    }
}
