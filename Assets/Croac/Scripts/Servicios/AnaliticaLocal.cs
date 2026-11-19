using System;
using System.IO;
using UnityEngine;

namespace Croac
{
    /// <summary>
    /// <see cref="IAnalytics"/> que escribe cada evento como una línea JSON en un fichero del móvil
    /// (<c>eventos.jsonl</c> en <c>persistentDataPath</c>). El usuario es un identificador al azar, no la cuenta ni el
    /// dispositivo; la sesión cambia cada vez que se abre el juego.
    /// </summary>
    public class AnaliticaLocal : IAnalytics
    {
        readonly string ruta;
        readonly string usuario;
        readonly string sesion = Guid.NewGuid().ToString("N");

        public static string RutaPorDefecto => Path.Combine(Application.persistentDataPath, "eventos.jsonl");

        public AnaliticaLocal(string ruta, string usuario)
        {
            this.ruta = ruta;
            this.usuario = usuario;
        }

        public void Registrar(Evento evento)
        {
            evento.t = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            evento.usuario = usuario;
            evento.sesion = sesion;
            File.AppendAllText(ruta, JsonUtility.ToJson(evento) + "\n");
        }
    }
}
