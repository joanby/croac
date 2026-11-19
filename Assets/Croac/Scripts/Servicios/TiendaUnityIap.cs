using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace Croac
{
    /// <summary>
    /// <see cref="IStore"/> con Unity IAP 5 (<c>PurchasingController</c>). En el editor, la tienda por defecto es la
    /// Fake Store: compra sin cuenta y sin dinero, con un diálogo de confirmación.
    /// </summary>
    public class TiendaUnityIap : IStore
    {
        static readonly List<ProductDefinition> Productos = new()
        {
            new ProductDefinition(Catalogo.Moscas, ProductType.Consumable),
            new ProductDefinition(Catalogo.SinAnuncios, ProductType.NonConsumable),
        };

        readonly string tienda = UnityIAPServices.GetDefaultStore();
        readonly PurchasingController iap;

        public event Action Preparada;
        public event Action<string, string> Entregar;
        public event Action<string> Fallo;

        public bool Lista { get; private set; }

        /// <summary>Se suscribe a todos los eventos antes de conectar: una compra pendiente puede llegar enseguida.</summary>
        public TiendaUnityIap()
        {
            iap = new PurchasingController(tienda);
            iap.OnStoreConnected += _ => iap.FetchProducts(Productos, tienda);
            iap.OnStoreDisconnected += (_, fallo) => Fallo?.Invoke(fallo.Message);
            iap.OnProductsFetched += AlRecibirProductos;
            iap.OnProductsFetchFailed += (_, fallo) => Fallo?.Invoke(fallo.FailureReason.ToString());
            iap.OnPurchasesFetched += (_, _) => { };
            iap.OnPurchasesFetchFailed += (_, fallo) => Fallo?.Invoke(fallo.Message);
            iap.OnPurchasePending += AlQuedarPendiente;
            iap.OnPurchaseConfirmed += AlConfirmar;
            iap.OnPurchaseFailed += (_, pedido) => Fallo?.Invoke(pedido.FailureReason.ToString());
            iap.OnPurchaseDeferred += (_, _) => Fallo?.Invoke("Pendiente de aprobación");
            _ = iap.Connect(tienda);
        }

        public string Precio(string producto) => iap.GetProductById(producto, tienda)?.metadata.localizedPriceString ?? "—";

        public void Comprar(string producto) => iap.PurchaseProduct(iap.GetProductById(producto, tienda));

        public void Restaurar() => iap.RestoreTransactions((_, bien, error) =>
        {
            if (!bien) Fallo?.Invoke(error);
        }, tienda);

        /// <summary>Con los productos ya en la tienda, pide las compras que quedaron sin confirmar.</summary>
        void AlRecibirProductos(string _, List<Product> productos)
        {
            Lista = true;
            Preparada?.Invoke();
            iap.FetchPurchases(tienda);
        }

        /// <summary>Primero se entrega y se guarda; después se confirma. Si la app muere en medio, la tienda lo repite.</summary>
        void AlQuedarPendiente(string _, PendingOrder pedido)
        {
            foreach (var linea in pedido.CartOrdered.Items())
                Entregar?.Invoke(linea.Product.definition.id, pedido.Info.TransactionID);
            iap.ConfirmPurchase(pedido);
        }

        void AlConfirmar(string _, Order pedido)
        {
            if (pedido is FailedOrder fallido) Fallo?.Invoke(fallido.FailureReason.ToString());
        }
    }
}
