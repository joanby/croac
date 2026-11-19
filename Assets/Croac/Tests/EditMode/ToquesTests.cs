using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Croac.Tests
{
    /// <summary>
    /// Las interacciones de <c>Croac.inputactions</c> con toques simulados: un toque corto salta, mantener el dedo hace
    /// el salto largo y dos toques seguidos repiten. <see cref="InputTestFixture"/> da un Input System vacío en cada test
    /// y deja mover el reloj (<c>currentTime</c>) para medir cuánto dura cada toque.
    /// </summary>
    public class ToquesTests : InputTestFixture
    {
        static readonly Vector2 Centro = new(540f, 960f);

        InputActionAsset acciones;
        InputAction saltar;
        InputAction saltoLargo;
        InputAction repetir;

        public override void Setup()
        {
            base.Setup();
            InputSystem.AddDevice<Touchscreen>();
            acciones = Object.Instantiate(AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Croac/Croac.inputactions"));
            saltar = acciones.FindAction("Juego/Saltar", true);
            saltoLargo = acciones.FindAction("Juego/SaltoLargo", true);
            repetir = acciones.FindAction("Juego/Repetir", true);
            acciones.Enable();
        }

        public override void TearDown()
        {
            acciones.Disable();
            Object.DestroyImmediate(acciones);
            base.TearDown();
        }

        [Test]
        public void UnToqueCortoSalta()
        {
            BeginTouch(1, Centro);
            currentTime += 0.1;
            EndTouch(1, Centro);

            Assert.That(saltar.WasPerformedThisFrame(), Is.True);
            Assert.That(saltoLargo.WasPerformedThisFrame(), Is.False);
        }

        [Test]
        public void MantenerElDedoHaceElSaltoLargoYNoElCorto()
        {
            BeginTouch(1, Centro);
            currentTime += 0.5;
            InputSystem.Update();
            Assert.That(saltoLargo.WasPerformedThisFrame(), Is.True);

            EndTouch(1, Centro);
            Assert.That(saltar.WasPerformedThisFrame(), Is.False);
        }

        [Test]
        public void DosToquesSeguidosRepiten()
        {
            BeginTouch(1, Centro);
            currentTime += 0.1;
            EndTouch(1, Centro);
            Assert.That(repetir.WasPerformedThisFrame(), Is.False);

            currentTime += 0.2;
            BeginTouch(2, Centro);
            currentTime += 0.1;
            EndTouch(2, Centro);
            Assert.That(repetir.WasPerformedThisFrame(), Is.True);
        }
    }
}
