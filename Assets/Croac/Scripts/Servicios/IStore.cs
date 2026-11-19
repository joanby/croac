using System;

namespace Croac
{
    /// <summary>
    /// Una tienda de compras dentro de la app, sin saber cuál: la de Unity IAP (<see cref="TiendaUnityIap"/>) o una
    /// falsa para los tests (<see cref="TiendaFalsa"/>).
    /// </summary>
    public interface IStore
    {
        /// <summary>Los productos ya tienen precio: se puede comprar.</summary>
        event Action Preparada;

        /// <summary>
        /// Hay que dar al jugador <c>producto</c> (el primer parámetro) del pedido <c>pedido</c> (el segundo). La
        /// tienda confirma la compra cuando todos los suscriptores han vuelto, así que hay que guardarlo aquí mismo.
        /// </summary>
        event Action<string, string> Entregar;

        /// <summary>Algo ha salido mal: la conexión, los productos o una compra. Lleva el motivo.</summary>
        event Action<string> Fallo;

        bool Lista { get; }

        /// <summary>El precio con su moneda, como lo da la tienda del sistema.</summary>
        string Precio(string producto);

        /// <summary>El precio como número, en la moneda de la tienda: lo que se apunta en la analítica.</summary>
        float Importe(string producto);

        void Comprar(string producto);

        /// <summary>Vuelve a entregar los no consumibles ya comprados (obligatorio en iOS: un botón «Restaurar»).</summary>
        void Restaurar();
    }
}
