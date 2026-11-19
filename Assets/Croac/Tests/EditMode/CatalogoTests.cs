using NUnit.Framework;

namespace Croac.Tests
{
    public class CatalogoTests
    {
        [Test]
        public void ElPaqueteDeMoscasSumaCincuenta()
        {
            var guardado = new Guardado { moscas = 3 };
            Assert.IsTrue(Catalogo.Entregar(guardado, Catalogo.Moscas, "pedido-1"));
            Assert.AreEqual(53, guardado.moscas);
        }

        [Test]
        public void ElMismoPedidoNoSeEntregaDosVeces()
        {
            var guardado = new Guardado();
            Catalogo.Entregar(guardado, Catalogo.Moscas, "pedido-1");
            Assert.IsFalse(Catalogo.Entregar(guardado, Catalogo.Moscas, "pedido-1"));
            Assert.IsTrue(Catalogo.Entregar(guardado, Catalogo.Moscas, "pedido-2"));
            Assert.AreEqual(2 * Catalogo.MoscasPorPaquete, guardado.moscas);
        }

        [Test]
        public void QuitarAnunciosSeRestauraSinDuplicarNada()
        {
            var guardado = new Guardado();
            Assert.IsTrue(Catalogo.Entregar(guardado, Catalogo.SinAnuncios, ""));
            Assert.IsFalse(Catalogo.Entregar(guardado, Catalogo.SinAnuncios, ""));
            Assert.IsTrue(guardado.sinAnuncios);
            Assert.AreEqual(0, guardado.moscas);
        }

        [Test]
        public void UnGuardadoAntiguoSinPedidosSeLeeConLaListaVacia()
        {
            var guardado = Guardado.DesdeJson("{\"record\":12,\"moscasTotales\":4,\"partidas\":2}");
            Assert.IsNotNull(guardado.pedidos);
            Assert.IsTrue(Catalogo.Entregar(guardado, Catalogo.Moscas, "pedido-1"));
        }
    }
}
