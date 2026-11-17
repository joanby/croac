namespace Croac
{
    /// <summary>Una fila de la charca: dónde empieza su nenúfar, a qué velocidad va y si lleva mosca.</summary>
    public readonly struct Fila
    {
        public readonly float X;
        public readonly float Velocidad;
        public readonly bool ConMosca;

        public Fila(float x, float velocidad, bool conMosca)
        {
            X = x;
            Velocidad = velocidad;
            ConMosca = conMosca;
        }
    }

    /// <summary>
    /// Genera las filas de nenúfares a partir de una semilla: la misma semilla da siempre la misma charca.
    /// </summary>
    public class GeneradorNenufares
    {
        readonly System.Random azar;
        readonly float limite;
        readonly float velocidadMinima;
        readonly float velocidadMaxima;
        readonly float probabilidadMosca;

        /// <param name="limite">Distancia máxima al centro, en unidades, a la que puede estar un nenúfar.</param>
        public GeneradorNenufares(int semilla, float limite, float velocidadMinima, float velocidadMaxima, float probabilidadMosca)
        {
            azar = new System.Random(semilla);
            this.limite = limite;
            this.velocidadMinima = velocidadMinima;
            this.velocidadMaxima = velocidadMaxima;
            this.probabilidadMosca = probabilidadMosca;
        }

        public GeneradorNenufares(Nivel nivel)
            : this(nivel.semilla, nivel.limite, nivel.velocidadMinima, nivel.velocidadMaxima, nivel.probabilidadMosca) { }

        /// <summary>La siguiente fila de la charca.</summary>
        public Fila Siguiente()
        {
            float x = Entre(-limite, limite);
            float velocidad = Entre(velocidadMinima, velocidadMaxima) * (azar.Next(2) == 0 ? -1f : 1f);
            bool conMosca = azar.NextDouble() < probabilidadMosca;
            return new Fila(x, velocidad, conMosca);
        }

        float Entre(float min, float max) => min + (max - min) * (float)azar.NextDouble();
    }
}
