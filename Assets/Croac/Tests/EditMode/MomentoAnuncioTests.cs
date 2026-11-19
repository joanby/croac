using NUnit.Framework;

namespace Croac.Tests
{
    public class MomentoAnuncioTests
    {
        [Test]
        public void SoloConLaPartidaTerminadaYLaRanaQuieta()
        {
            Assert.IsTrue(MomentoAnuncio.Permitido(terminada: true, enElAire: false, yaRevivida: false));
            Assert.IsFalse(MomentoAnuncio.Permitido(terminada: false, enElAire: false, yaRevivida: false));
        }

        [Test]
        public void NuncaAMitadDeUnSalto()
        {
            Assert.IsFalse(MomentoAnuncio.Permitido(terminada: true, enElAire: true, yaRevivida: false));
            Assert.IsFalse(MomentoAnuncio.Permitido(terminada: false, enElAire: true, yaRevivida: false));
        }

        [Test]
        public void UnaVezPorPartida()
        {
            Assert.IsFalse(MomentoAnuncio.Permitido(terminada: true, enElAire: false, yaRevivida: true));
        }
    }
}
