using System.IO;
using NUnit.Framework;

namespace Croac.Tests
{
    public class CicloVidaTests
    {
        int guardados;
        CicloVida ciclo;

        [SetUp]
        public void Preparar()
        {
            guardados = 0;
            ciclo = new CicloVida(() => guardados++);
        }

        [Test]
        public void GuardaAlIrseASegundoPlanoYNoAlVolver()
        {
            ciclo.AlPausar(true);
            ciclo.AlPausar(false);
            Assert.AreEqual(1, guardados);
        }

        [Test]
        public void GuardaAlPerderElFocoYNoAlRecuperarlo()
        {
            ciclo.AlCambiarFoco(false);
            ciclo.AlCambiarFoco(true);
            Assert.AreEqual(1, guardados);
        }

        [Test]
        public void GuardaSiFaltaMemoria()
        {
            ciclo.AlFaltarMemoria();
            Assert.AreEqual(1, guardados);
        }

        [Test]
        public void ElRecordDeUnaPartidaAMediasSobreviveAUnaPausa()
        {
            string ruta = Path.Combine(Path.GetTempPath(), $"croac-{System.Guid.NewGuid():N}.json");
            try
            {
                new Guardado { record = 4, partidas = 3 }.Guardar(ruta);
                var puntuacion = new Puntuacion();
                puntuacion.Salto();
                puntuacion.Mosca();
                var pausa = new CicloVida(() => Guardado.GuardarRecord(puntuacion.Puntos, ruta));

                pausa.AlPausar(true);

                var leido = Guardado.Cargar(ruta);
                Assert.AreEqual(6, leido.record);
                Assert.AreEqual(3, leido.partidas, "una partida a medias no cuenta como jugada");
            }
            finally
            {
                File.Delete(ruta);
            }
        }

        [Test]
        public void UnaPausaNoBajaElRecord()
        {
            string ruta = Path.Combine(Path.GetTempPath(), $"croac-{System.Guid.NewGuid():N}.json");
            try
            {
                new Guardado { record = 40 }.Guardar(ruta);
                new CicloVida(() => Guardado.GuardarRecord(7, ruta)).AlPausar(true);
                Assert.AreEqual(40, Guardado.Cargar(ruta).record);
            }
            finally
            {
                File.Delete(ruta);
            }
        }
    }
}
