using NUnit.Framework;

namespace Croac.Tests
{
    public class GuardadoTests
    {
        [Test]
        public void SobreviveAlViajeDeIdaYVueltaPorJson()
        {
            var guardado = new Guardado { record = 37, moscasTotales = 12, partidas = 5 };
            var leido = Guardado.DesdeJson(guardado.AJson());
            Assert.AreEqual(37, leido.record);
            Assert.AreEqual(12, leido.moscasTotales);
            Assert.AreEqual(5, leido.partidas);
        }

        [Test]
        public void SinTextoDaUnGuardadoVacio()
        {
            var guardado = Guardado.DesdeJson(null);
            Assert.AreEqual(0, guardado.record);
            Assert.AreEqual(0, guardado.partidas);
        }

        [Test]
        public void RegistrarSoloCambiaElRecordSiLoSupera()
        {
            var guardado = new Guardado { record = 10 };
            var puntuacion = new Puntuacion();
            puntuacion.Mosca();
            Assert.IsFalse(guardado.Registrar(puntuacion));
            Assert.AreEqual(10, guardado.record);
            puntuacion.Mosca();
            puntuacion.Mosca();
            Assert.IsTrue(guardado.Registrar(puntuacion));
            Assert.AreEqual(15, guardado.record);
            Assert.AreEqual(2, guardado.partidas);
            Assert.AreEqual(4, guardado.moscasTotales);
        }
    }
}
