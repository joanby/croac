namespace Croac
{
    /// <summary>Puntos de una partida: 1 por salto y 5 por mosca.</summary>
    public class Puntuacion
    {
        public const int PorSalto = 1;
        public const int PorMosca = 5;

        public int Puntos { get; private set; }
        public int Saltos { get; private set; }
        public int Moscas { get; private set; }

        /// <summary>Cuenta un salto que acaba en un nenúfar.</summary>
        public void Salto()
        {
            Saltos++;
            Puntos += PorSalto;
        }

        /// <summary>Cuenta una mosca atrapada.</summary>
        public void Mosca()
        {
            Moscas++;
            Puntos += PorMosca;
        }
    }
}
