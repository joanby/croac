using NUnit.Framework;

namespace Croac.Tests
{
    public class GeneradorNenufaresTests
    {
        const float Limite = 3.2f;

        static GeneradorNenufares Generador(int semilla) => new(semilla, Limite, 0.8f, 2f, 0.3f);

        [Test]
        public void LaMismaSemillaDaLaMismaCharca()
        {
            var a = Generador(42);
            var b = Generador(42);
            for (int i = 0; i < 100; i++)
            {
                var fa = a.Siguiente();
                var fb = b.Siguiente();
                Assert.AreEqual(fa.X, fb.X);
                Assert.AreEqual(fa.Velocidad, fb.Velocidad);
                Assert.AreEqual(fa.ConMosca, fb.ConMosca);
            }
        }

        [Test]
        public void OtraSemillaDaOtraCharca()
        {
            var a = Generador(1);
            var b = Generador(2);
            bool distinta = false;
            for (int i = 0; i < 10; i++) distinta |= a.Siguiente().X != b.Siguiente().X;
            Assert.IsTrue(distinta);
        }

        [Test]
        public void LasFilasRespetanLimiteYVelocidad()
        {
            var generador = Generador(7);
            int moscas = 0;
            for (int i = 0; i < 1000; i++)
            {
                var fila = generador.Siguiente();
                Assert.That(fila.X, Is.InRange(-Limite, Limite));
                Assert.That(System.Math.Abs(fila.Velocidad), Is.InRange(0.8f, 2f));
                if (fila.ConMosca) moscas++;
            }
            Assert.That(moscas, Is.InRange(200, 400));
        }

        [Test]
        public void LaSemilla42EmpiezaSiempreIgual()
        {
            var fila = Generador(42).Siguiente();
            Assert.AreEqual(1.0759f, fila.X, 1e-3f);
        }
    }
}
