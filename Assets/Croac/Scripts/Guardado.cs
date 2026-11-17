using System;
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

        public static string Ruta => Path.Combine(Application.persistentDataPath, "guardado.json");

        /// <summary>Suma una partida terminada. Devuelve <c>true</c> si es un récord nuevo.</summary>
        public bool Registrar(Puntuacion puntuacion)
        {
            partidas++;
            moscasTotales += puntuacion.Moscas;
            if (puntuacion.Puntos <= record) return false;
            record = puntuacion.Puntos;
            return true;
        }

        public string AJson() => JsonUtility.ToJson(this);

        /// <summary>Un guardado desde su JSON; si no hay texto, uno vacío.</summary>
        public static Guardado DesdeJson(string json) =>
            string.IsNullOrEmpty(json) ? new Guardado() : JsonUtility.FromJson<Guardado>(json);

        public static Guardado Cargar() => DesdeJson(File.Exists(Ruta) ? File.ReadAllText(Ruta) : null);

        public void Guardar() => File.WriteAllText(Ruta, AJson());
    }
}
