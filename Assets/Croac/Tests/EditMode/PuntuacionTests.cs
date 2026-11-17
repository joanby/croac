using NUnit.Framework;

namespace Croac.Tests
{
    public class PuntuacionTests
    {
        [Test]
        public void EmpiezaACero()
        {
            var puntuacion = new Puntuacion();
            Assert.AreEqual(0, puntuacion.Puntos);
        }

        [Test]
        public void CadaSaltoSumaUnPunto()
        {
            var puntuacion = new Puntuacion();
            puntuacion.Salto();
            puntuacion.Salto();
            Assert.AreEqual(2 * Puntuacion.PorSalto, puntuacion.Puntos);
            Assert.AreEqual(2, puntuacion.Saltos);
        }

        [Test]
        public void UnaMoscaValeCincoPuntos()
        {
            var puntuacion = new Puntuacion();
            puntuacion.Salto();
            puntuacion.Mosca();
            Assert.AreEqual(6, puntuacion.Puntos);
            Assert.AreEqual(1, puntuacion.Moscas);
        }
    }
}
