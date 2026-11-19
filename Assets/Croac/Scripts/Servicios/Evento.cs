using System;

namespace Croac
{
    /// <summary>
    /// Un evento de analítica: <c>level_start</c>, <c>level_end</c>, <c>purchase</c> o <c>ad_reward</c>, con los campos
    /// que le tocan. Es una fila del panel de métricas: las mismas columnas que el conjunto ficticio de la 04.6.
    /// </summary>
    [Serializable]
    public class Evento
    {
        public string t;
        public string usuario;
        public string sesion;
        public string evento;
        public string nivel;
        public int puntos;
        public string producto;
        public float ingreso;
    }
}
