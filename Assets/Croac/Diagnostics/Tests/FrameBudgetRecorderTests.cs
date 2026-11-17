using System.IO;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace Croac.Diagnostics.Tests
{
    public class FrameBudgetRecorderTests
    {
        [Test]
        public void LeerLosContadoresYAnotarNoAsignaMemoria()
        {
            using var contadores = new ContadoresFrame();
            var bufer = new BuferPresupuesto(1000);
            bufer.Anotar(contadores.Muestrear(1));

            Assert.That(() =>
            {
                for (int i = 0; i < 500; i++) bufer.Anotar(contadores.Muestrear(1));
            }, Is.Not.AllocatingGCMemory());
        }

        [Test]
        public void ElCsvTieneLaCabeceraYUnaFilaPorFrameConPuntoDecimal()
        {
            var bufer = new BuferPresupuesto(2);
            bufer.Anotar(new MuestraFrame { frame = 10, cpuMainMs = 16.5f, gcAllocBytes = 2048, drawCalls = 12, totalUsedMb = 300.25f, nivel = 1 });
            bufer.Anotar(new MuestraFrame { frame = 11, cpuMainMs = 8.25f, canvasRebuilds = 1, nivel = 2 });
            bufer.Anotar(new MuestraFrame { frame = 12 });
            var salida = new StringWriter();

            bufer.EscribirCsv(salida);

            var lineas = salida.ToString().TrimEnd().Split('\n');
            Assert.AreEqual(3, lineas.Length, "cabecera y dos filas: la tercera no cabe");
            Assert.AreEqual(BuferPresupuesto.Cabecera, lineas[0].TrimEnd('\r'));
            Assert.AreEqual("10,16.5,0,0,2048,0,0,12,0,0,0,0,300.25,1", lineas[1].TrimEnd('\r'));
            Assert.AreEqual("11,8.25,0,0,0,0,0,0,0,0,1,0,0,2", lineas[2].TrimEnd('\r'));
            Assert.AreEqual(14, lineas[0].Split(',').Length);
        }

        [Test]
        public void LosArgumentosSeLeenPorNombre()
        {
            var args = new[] { "Croac", "-croacBudget", "a.csv", "-croacFrames", "600" };
            Assert.AreEqual("a.csv", FrameBudgetRecorder.Argumento(args, "-croacBudget"));
            Assert.AreEqual("600", FrameBudgetRecorder.Argumento(args, "-croacFrames"));
            Assert.IsNull(FrameBudgetRecorder.Argumento(args, "-croacWarmup"));
        }
    }
}
