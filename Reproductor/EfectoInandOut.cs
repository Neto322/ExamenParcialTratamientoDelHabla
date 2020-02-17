using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Reproductor
{
    class EfectoInandOut : ISampleProvider
    {
        private ISampleProvider fuente;

        private int muestrasLeidas = 0;
        private float segundosTranscurridos = 0;
        private float duracion;
        private float inicio;
        private float duracionfadeout;

        public EfectoInandOut(ISampleProvider fuente,
          float duracion,float inicio,float duracionfadeout)
        {
            this.fuente = fuente;
            this.duracion = duracion;
            this.inicio = inicio;
            this.duracionfadeout = duracionfadeout;
        }
        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = fuente.Read(buffer, offset, count);

            muestrasLeidas += read;
            segundosTranscurridos =
                (float)muestrasLeidas /
                (float)fuente.WaveFormat.SampleRate /
                (float)fuente.WaveFormat.Channels;

            if (segundosTranscurridos <= duracion)
            {
                //Aplicar el efecto
                float factorEscala = segundosTranscurridos / duracion;
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *=
                        factorEscala;
                }
            }

            if (segundosTranscurridos >= inicio &&
              segundosTranscurridos <= inicio + duracion)
            {
                //Aplicar el efecto
                float factorEscala =
                    1 - ((segundosTranscurridos - inicio) /
                        duracion);
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *=
                        factorEscala;
                }
            }
            else if (segundosTranscurridos >= inicio + duracion)
            {
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] = 0.0f;
                }
            }

            return read;
        }
    }
}
