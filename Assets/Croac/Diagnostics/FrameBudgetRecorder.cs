using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Croac.Diagnostics
{
    /// <summary>Las cifras de un frame, en las unidades de las columnas del CSV.</summary>
    public struct MuestraFrame
    {
        public int frame;
        public float cpuMainMs;
        public float cpuRenderMs;
        public float gpuMs;
        public long gcAllocBytes;
        public long gcAllocCount;
        public long setPassCalls;
        public long drawCalls;
        public float uiLayoutMs;
        public float uiRenderMs;
        public long canvasRebuilds;
        public float textureMb;
        public float totalUsedMb;
        public int nivel;
    }

    /// <summary>
    /// Guarda las muestras en un array que se reserva entero al crearlo, así que <see cref="Anotar"/> no asigna
    /// memoria: si lo hiciera, el grabador ensuciaría la columna <c>gc_alloc_bytes</c> que quiere medir.
    /// El CSV se escribe una sola vez, al final.
    /// </summary>
    public class BuferPresupuesto
    {
        public const string Cabecera =
            "frame,cpu_main_ms,cpu_render_ms,gpu_ms,gc_alloc_bytes,gc_alloc_count,setpass_calls,draw_calls," +
            "ui_layout_ms,ui_render_ms,canvas_rebuilds,texture_mb,total_used_mb,nivel";

        readonly MuestraFrame[] muestras;

        public BuferPresupuesto(int frames) => muestras = new MuestraFrame[frames];

        public int Cuenta { get; private set; }

        public bool Lleno => Cuenta == muestras.Length;

        /// <summary>Añade una muestra. Con el búfer lleno, no hace nada.</summary>
        public void Anotar(in MuestraFrame muestra)
        {
            if (Lleno) return;
            muestras[Cuenta++] = muestra;
        }

        /// <summary>La cabecera y una fila por muestra, con punto decimal sea cual sea el idioma del sistema.</summary>
        public void EscribirCsv(TextWriter salida)
        {
            var c = CultureInfo.InvariantCulture;
            salida.WriteLine(Cabecera);
            for (int i = 0; i < Cuenta; i++)
            {
                var m = muestras[i];
                salida.WriteLine(string.Join(",",
                    m.frame.ToString(c), m.cpuMainMs.ToString("0.###", c), m.cpuRenderMs.ToString("0.###", c),
                    m.gpuMs.ToString("0.###", c), m.gcAllocBytes.ToString(c), m.gcAllocCount.ToString(c),
                    m.setPassCalls.ToString(c), m.drawCalls.ToString(c),
                    m.uiLayoutMs.ToString("0.###", c), m.uiRenderMs.ToString("0.###", c), m.canvasRebuilds.ToString(c),
                    m.textureMb.ToString("0.##", c), m.totalUsedMb.ToString("0.##", c), m.nivel.ToString(c)));
            }
        }
    }

    /// <summary>
    /// Los <see cref="ProfilerRecorder"/> de cada columna. Leerlos no asigna memoria: son structs que guardan el
    /// valor del último frame terminado. Nombres y categorías comprobados en el player de 6000.6.8f1: los tiempos
    /// de CPU y GPU son los del FrameTimingManager (Render), las asignaciones y la memoria los de Memory, las
    /// llamadas de dibujo y los SetPass los de Render, y la UI los marcadores <c>Layout</c> (UI Layout), <c>Render</c> y
    /// <c>Canvas.BuildBatch</c> (UI Render). <c>canvas_rebuilds</c> es cuántas veces se rehízo el lote de un
    /// Canvas en el frame. No hay columna <c>batches</c>: 6000.6 ya no tiene el contador «Batches Count» de Render
    /// (en Statistics se llama Draw Calls) y el único con ese nombre es el de UI Toolkit, que aquí siempre vale 0.
    /// </summary>
    public class ContadoresFrame : IDisposable
    {
        const float Mb = 1024f * 1024f;
        const float NsPorMs = 1e6f;

        ProfilerRecorder cpuMain, cpuRender, gpu, gcBytes, gcCount, setPass, drawCalls;
        ProfilerRecorder uiLayout, uiRender, canvasRebuilds, texturas, memoriaTotal;

        static List<ProfilerRecorderHandle> disponibles;

        /// <summary>Lanza <see cref="InvalidOperationException"/> si falta alguno de los contadores.</summary>
        public ContadoresFrame()
        {
            cpuMain = Iniciar(ProfilerCategory.Render, "CPU Main Thread Frame Time");
            cpuRender = Iniciar(ProfilerCategory.Render, "CPU Render Thread Frame Time");
            gpu = Iniciar(ProfilerCategory.Render, "GPU Frame Time");
            gcBytes = Iniciar(ProfilerCategory.Memory, "GC Allocated In Frame");
            gcCount = Iniciar(ProfilerCategory.Memory, "GC Allocation In Frame Count");
            setPass = Iniciar(ProfilerCategory.Render, "SetPass Calls Count");
            drawCalls = Iniciar(ProfilerCategory.Render, "Draw Calls Count");
            uiLayout = Iniciar(new ProfilerCategory("UI Layout"), "Layout");
            uiRender = Iniciar(new ProfilerCategory("UI Render"), "Render");
            canvasRebuilds = Iniciar(new ProfilerCategory("UI Render"), "Canvas.BuildBatch");
            texturas = Iniciar(ProfilerCategory.Memory, "Texture Memory");
            memoriaTotal = Iniciar(ProfilerCategory.Memory, "Total Used Memory");
        }

        static ProfilerRecorder Iniciar(ProfilerCategory categoria, string nombre)
        {
            Exigir(categoria, nombre);
            return ProfilerRecorder.StartNew(categoria, nombre);
        }

        /// <summary>
        /// Comprueba que el contador o marcador existe en esa categoría y, si no, lanza
        /// <see cref="InvalidOperationException"/>. Sin esta comprobación, ProfilerRecorder se engancha a otro con el
        /// mismo nombre (o a ninguno) y la columna sale a 0 sin avisar.
        /// </summary>
        public static void Exigir(ProfilerCategory categoria, string nombre)
        {
            if (disponibles == null)
            {
                disponibles = new List<ProfilerRecorderHandle>();
                ProfilerRecorderHandle.GetAvailable(disponibles);
            }
            foreach (var handle in disponibles)
            {
                var descripcion = ProfilerRecorderHandle.GetDescription(handle);
                if (descripcion.Name == nombre && descripcion.Category.Name == categoria.Name) return;
            }
            throw new InvalidOperationException($"el contador '{nombre}' ({categoria.Name}) no existe en esta versión o plataforma");
        }

        /// <summary>Las cifras del último frame terminado.</summary>
        public MuestraFrame Muestrear(int nivel) => new()
        {
            frame = Time.frameCount - 1,
            cpuMainMs = (float)(cpuMain.LastValueAsDouble / NsPorMs),
            cpuRenderMs = (float)(cpuRender.LastValueAsDouble / NsPorMs),
            gpuMs = (float)(gpu.LastValueAsDouble / NsPorMs),
            gcAllocBytes = gcBytes.LastValue,
            gcAllocCount = gcCount.LastValue,
            setPassCalls = setPass.LastValue,
            drawCalls = drawCalls.LastValue,
            uiLayoutMs = uiLayout.LastValue / NsPorMs,
            uiRenderMs = uiRender.LastValue / NsPorMs,
            canvasRebuilds = canvasRebuilds.Count > 0 ? canvasRebuilds.GetSample(0).Count : 0,
            textureMb = texturas.LastValue / Mb,
            totalUsedMb = memoriaTotal.LastValue / Mb,
            nivel = nivel,
        };

        public void Dispose()
        {
            cpuMain.Dispose(); cpuRender.Dispose(); gpu.Dispose(); gcBytes.Dispose(); gcCount.Dispose();
            setPass.Dispose(); drawCalls.Dispose(); uiLayout.Dispose(); uiRender.Dispose();
            canvasRebuilds.Dispose(); texturas.Dispose(); memoriaTotal.Dispose();
        }
    }

    /// <summary>
    /// Graba el presupuesto de cada frame en un CSV (columnas en <see cref="BuferPresupuesto.Cabecera"/>, contadores
    /// en <see cref="ContadoresFrame"/>). Se activa solo si el juego arranca con estos argumentos:
    /// <list type="bullet">
    /// <item><c>-croacBudget &lt;csv&gt;</c>: dónde escribir. Una ruta relativa va dentro de <c>persistentDataPath</c>
    /// (así se usa en un móvil, y se recoge con <c>adb pull</c>).</item>
    /// <item><c>-croacFrames &lt;n&gt;</c>: cuántos frames grabar (600 si no se dice).</item>
    /// <item><c>-croacWarmup &lt;n&gt;</c>: cuántos frames dejar pasar antes de empezar (120 si no se dice).</item>
    /// </list>
    /// Cada fila es un frame: <c>frame</c> es el frame terminado que se ha leído, y <c>nivel</c> cuenta las veces
    /// que se ha cargado una escena de juego (una con un <see cref="IJugador"/>; 1 es la primera). Al llenar el
    /// búfer escribe el CSV y cierra el juego. Si falta algún contador, el CSV tiene una sola línea que empieza por
    /// <c>ERROR</c> y el juego se cierra con código 1: una columna a 0 sin avisar es peor que no medir.
    /// </summary>
    public class FrameBudgetRecorder : MonoBehaviour
    {
        string ruta;
        int calentamiento;
        BuferPresupuesto bufer;
        ContadoresFrame contadores;
        int nivel;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Activar()
        {
            var args = Environment.GetCommandLineArgs();
            string csv = Argumento(args, "-croacBudget");
            if (csv == null) return;
            var go = new GameObject(nameof(FrameBudgetRecorder));
            DontDestroyOnLoad(go);
            go.AddComponent<FrameBudgetRecorder>().Configurar(
                Path.Combine(Application.persistentDataPath, csv),
                int.Parse(Argumento(args, "-croacFrames") ?? "600", CultureInfo.InvariantCulture),
                int.Parse(Argumento(args, "-croacWarmup") ?? "120", CultureInfo.InvariantCulture));
        }

        /// <summary>El valor que sigue a <paramref name="nombre"/> en los argumentos, o <c>null</c>.</summary>
        public static string Argumento(string[] args, string nombre)
        {
            int i = Array.IndexOf(args, nombre);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
        }

        void Configurar(string rutaCsv, int frames, int warmup)
        {
            ruta = rutaCsv;
            calentamiento = warmup;
            bufer = new BuferPresupuesto(frames);
            Application.runInBackground = true;
            try
            {
                contadores = new ContadoresFrame();
            }
            catch (InvalidOperationException error)
            {
                File.WriteAllText(ruta, $"ERROR {error.Message}\n");
                Debug.LogError($"FrameBudgetRecorder: {error.Message}");
                enabled = false;
                Application.Quit(1);
                return;
            }
            SceneManager.sceneLoaded += AlCargarEscena;
            Debug.Log($"FrameBudgetRecorder: {frames} frames tras {warmup} de calentamiento → {ruta}");
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            contadores?.Dispose();
        }

        void AlCargarEscena(Scene escena, LoadSceneMode _)
        {
            foreach (var raiz in escena.GetRootGameObjects())
            {
                if (raiz.GetComponentInChildren<IJugador>() == null) continue;
                nivel++;
                return;
            }
        }

        void Update()
        {
            if (calentamiento > 0)
            {
                calentamiento--;
                return;
            }
            bufer.Anotar(contadores.Muestrear(nivel));
            if (!bufer.Lleno) return;
            using (var salida = new StreamWriter(ruta)) bufer.EscribirCsv(salida);
            Debug.Log($"FrameBudgetRecorder: {bufer.Cuenta} frames en {ruta}");
            enabled = false;
            Application.Quit();
        }
    }
}
