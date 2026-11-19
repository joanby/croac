using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Croac.Tests
{
    public class AnaliticaTests
    {
        string ruta;

        [SetUp]
        public void Preparar() => ruta = Path.Combine(Path.GetTempPath(), $"croac-eventos-{System.Guid.NewGuid():N}.jsonl");

        [TearDown]
        public void Limpiar() => File.Delete(ruta);

        [Test]
        public void CadaEventoEsUnaLineaConUsuarioYSesion()
        {
            var analitica = new AnaliticaLocal(ruta, "u1");
            analitica.Registrar(new Evento { evento = "level_start", nivel = "LaCharca" });
            analitica.Registrar(new Evento { evento = "level_end", nivel = "LaCharca", puntos = 12 });
            var lineas = File.ReadAllLines(ruta);
            Assert.AreEqual(2, lineas.Length);
            var fin = JsonUtility.FromJson<Evento>(lineas[1]);
            Assert.AreEqual("level_end", fin.evento);
            Assert.AreEqual(12, fin.puntos);
            Assert.AreEqual("u1", fin.usuario);
            Assert.AreEqual(JsonUtility.FromJson<Evento>(lineas[0]).sesion, fin.sesion);
        }

        [Test]
        public void UnGuardadoAntiguoNoTieneConsentimiento()
        {
            var guardado = Guardado.DesdeJson("{\"record\":12}");
            Assert.AreEqual(Consentimiento.SinPreguntar, guardado.consentimiento);
        }
    }
}
