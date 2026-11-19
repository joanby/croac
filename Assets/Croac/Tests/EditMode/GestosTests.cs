using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Croac.Tests
{
    /// <summary>
    /// <see cref="Gestos"/> con toques simulados: deslizar el dedo cambia de carril, un toque quieto no, y separar dos
    /// dedos da el factor de zoom. Cada <c>BeginTouch</c>, <c>MoveTouch</c> o <c>EndTouch</c> es un frame del Input System.
    /// </summary>
    public class GestosTests : InputTestFixture
    {
        const float Umbral = 100f;

        Gestos gestos;

        public override void Setup()
        {
            base.Setup();
            InputSystem.AddDevice<Touchscreen>();
            gestos = new Gestos(Umbral);
        }

        public override void TearDown()
        {
            gestos.Dispose();
            base.TearDown();
        }

        [Test]
        public void DeslizarALaDerechaCambiaDeCarrilAlLevantarElDedo()
        {
            BeginTouch(1, new Vector2(300f, 900f));
            MoveTouch(1, new Vector2(450f, 910f));
            gestos.Actualizar();
            Assert.That(gestos.Deslizamiento, Is.EqualTo(0));
            Assert.That(gestos.Moviendose, Is.True);

            EndTouch(1, new Vector2(500f, 910f));
            gestos.Actualizar();
            Assert.That(gestos.Deslizamiento, Is.EqualTo(1));
        }

        [Test]
        public void DeslizarALaIzquierdaDaMenosUno()
        {
            BeginTouch(1, new Vector2(700f, 900f));
            MoveTouch(1, new Vector2(550f, 900f));
            EndTouch(1, new Vector2(500f, 900f));
            gestos.Actualizar();
            Assert.That(gestos.Deslizamiento, Is.EqualTo(-1));
        }

        [Test]
        public void UnToqueQuietoNoEsUnDeslizamiento()
        {
            BeginTouch(1, new Vector2(500f, 900f));
            EndTouch(1, new Vector2(505f, 902f));
            gestos.Actualizar();
            Assert.That(gestos.Deslizamiento, Is.EqualTo(0));
            Assert.That(gestos.Moviendose, Is.False);
        }

        [Test]
        public void SepararDosDedosDaElFactorDeZoom()
        {
            BeginTouch(1, new Vector2(400f, 900f));
            BeginTouch(2, new Vector2(600f, 900f));
            gestos.Actualizar();
            Assert.That(gestos.Pellizco, Is.EqualTo(1f));

            MoveTouch(2, new Vector2(800f, 900f));
            gestos.Actualizar();
            Assert.That(gestos.Pellizco, Is.EqualTo(2f).Within(0.001f));
        }
    }
}
