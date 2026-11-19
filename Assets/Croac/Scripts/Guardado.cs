using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Croac
{
    /// <summary>Lo que se guarda entre partidas, en JSON dentro de <c>persistentDataPath</c>.</summary>
    [Serializable]
    public class Guardado
    {
        public int record;
        public int moscasTotales;
        public int partidas;
        public int moscas;
        public bool sinAnuncios;
        public List<string> pedidos = new();

        public static string Ruta => Path.Combine(Application.persistentDataPath, "guardado.json");

        /// <summary>Suma una partida terminada. Devuelve <c>true</c> si es un récord nuevo.</summary>
        public bool Registrar(Puntuacion puntuacion)
        {
            partidas++;
            moscasTotales += puntuacion.Moscas;
            moscas += puntuacion.Moscas;
            return ActualizarRecord(puntuacion.Puntos);
        }

        /// <summary>Sube el récord si <paramref name="puntos"/> lo supera. Devuelve <c>true</c> si lo ha cambiado.</summary>
        public bool ActualizarRecord(int puntos)
        {
            if (puntos <= record) return false;
            record = puntos;
            return true;
        }

        public string AJson() => JsonUtility.ToJson(this);

        /// <summary>Un guardado desde su JSON; si no hay texto, uno vacío.</summary>
        public static Guardado DesdeJson(string json) =>
            string.IsNullOrEmpty(json) ? new Guardado() : JsonUtility.FromJson<Guardado>(json);

        public static Guardado Cargar(string ruta) => DesdeJson(File.Exists(ruta) ? File.ReadAllText(ruta) : null);

        public static Guardado Cargar() => Cargar(Ruta);

        public void Guardar(string ruta) => File.WriteAllText(ruta, AJson());

        public void Guardar() => Guardar(Ruta);

        /// <summary>
        /// Guarda el récord de una partida que aún no ha terminado, sin contarla como jugada: es lo que se hace
        /// al irse a segundo plano, por si el sistema mata la app.
        /// </summary>
        public static void GuardarRecord(int puntos, string ruta)
        {
            var guardado = Cargar(ruta);
            if (guardado.ActualizarRecord(puntos)) guardado.Guardar(ruta);
        }
    }
}
