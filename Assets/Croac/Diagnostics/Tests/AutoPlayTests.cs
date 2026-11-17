using System.Collections.Generic;
using NUnit.Framework;

namespace Croac.Diagnostics.Tests
{
    public class AutoPlayTests
    {
        /// <summary>Una partida de mentira: el nenúfar siguiente oscila y cada salto dura 13 frames.</summary>
        class JugadorFalso : IJugador
        {
            public int Frame;
            int aterrizaEn;

            public bool PuedeSaltar => Frame >= aterrizaEn;
            public bool Terminada => false;
            public float DesvioSiguiente => 2f * (float)System.Math.Sin(Frame * 0.07);
            public void Saltar() => aterrizaEn = Frame + 13;
            public void Reiniciar() => aterrizaEn = Frame + 1;
        }

        static List<string> Jugar(int semilla, int frames)
        {
            var bot = new Bot(semilla, AutoPlay.SaltosPorNivel);
            var jugador = new JugadorFalso();
            var acciones = new List<string>();
            for (; jugador.Frame < frames; jugador.Frame++)
            {
                var accion = bot.Decidir(jugador);
                if (accion == AccionBot.Esperar) continue;
                acciones.Add($"{jugador.Frame}:{accion}");
                if (accion == AccionBot.Saltar) jugador.Saltar();
                else jugador.Reiniciar();
            }
            return acciones;
        }

        [Test]
        public void LaMismaSemillaDaLosMismosSaltos()
        {
            var primera = Jugar(7, 3000);
            Assert.Greater(primera.Count, 20);
            CollectionAssert.AreEqual(primera, Jugar(7, 3000));
        }

        [Test]
        public void OtraSemillaDaOtrosSaltos()
        {
            CollectionAssert.AreNotEqual(Jugar(7, 3000), Jugar(8, 3000));
        }

        [Test]
        public void PideOtroNivelCadaTresSaltos()
        {
            var acciones = Jugar(7, 3000);
            int saltos = 0;
            foreach (var accion in acciones)
            {
                if (accion.EndsWith("Saltar")) saltos++;
                else
                {
                    StringAssert.EndsWith("CambiarNivel", accion);
                    Assert.AreEqual(AutoPlay.SaltosPorNivel, saltos);
                    saltos = 0;
                }
            }
        }
    }
}
