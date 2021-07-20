using System;
using System.Linq;
using System.Collections.Generic;

using PersonalizerDemo.Servicios;

namespace PersonalizerDemo
{
    class Program
    {
        private static string LeerCaracter()
        {
            return Console.ReadKey().Key.ToString().Last().ToString().ToUpper();
        }

        private static string ObtenerHoraActual()
        {
            var horas = new string[] { "mañana", "tarde", "noche" };

            Console.WriteLine("\n¿Qué hora es (ingresa número)? 1. mañana 2. tarde 3. noche");
            if (!int.TryParse(LeerCaracter(), out int indice) || indice < 1 || indice > horas.Length)
            {
                Console.WriteLine("\nEl valor ingresado no es válido. Se seleccionará " + horas[0] + ".");
                indice = 1;
            }

            return horas[indice - 1];
        }

        private static string ObtenerSaborPreferido()
        {
            var sabores = new string[] { "salado", "dulce" };

            Console.WriteLine("\n¿Qué tipo de comida prefieres (ingresa número)? 1. salado 2. dulce");
            if (!int.TryParse(LeerCaracter(), out int indice) || indice < 1 || indice > sabores.Length)
            {
                Console.WriteLine("\nEl valor ingresado no es válido. Se seleccionará " + sabores[0] + ".");
                indice = 1;
            }

            return sabores[indice - 1];
        }

        static void Main(string[] args)
        {
            var iteracion = 1;
            var continuar = true;

            // Obtén la lista de acciones a elegir del servicio de Personalizer junto con sus características.
            var acciones = ServicioMenu.ObtenerAcciones();

            do
            {
                Console.WriteLine("\nIteracion: " + iteracion++);

                // Solicita la información de contexto del usuario.
                var horaDelDia = ObtenerHoraActual();
                var saborPreferido = ObtenerSaborPreferido();

                // Crea el contexto actual en base a los datos seleccionados.
                var contextoUsuario = new List<object>() 
                {
                    new { hora = horaDelDia },
                    new { sabor = saborPreferido }
                };

                // Es posible excluir una acción al momento de puntuar las recomendaciones de Personalizer.
                // Esto simula una regla de negocio para forzar la acción a ser ignorada
                // El API devuelve un valor 0 para acciones ignoradas.
                var accionesIgnoradas = horaDelDia != "mañana" ? new List<string> { "jugo" } : new List<string>();

                // Obtén la sugerencia del servicio en base al contexto
                var recomendacion = ServicioPersonalizer.ObtenerRecomendacion(acciones, contextoUsuario, accionesIgnoradas);
                Console.WriteLine("\nEl servicio de Personalizer te recomienda: " + recomendacion.RewardActionId);

                Console.WriteLine("\nComo dato adicional, el servicio de Personalizer clasificó las acciones con las siguientes probabilidades:");
                foreach (var sugerencias in recomendacion.Ranking)
                {
                    Console.WriteLine(sugerencias.Id + " " + sugerencias.Probability);
                }

                Console.WriteLine("¿Te agradó la sugerencia? (S/N)");
                var respuestaUsuario = LeerCaracter();

                var retroalimentacion = 0.0f;

                if (respuestaUsuario == "S")
                {
                    retroalimentacion = 1;
                    Console.WriteLine("\n¡Excelente! Disfruta tu comida =)");
                }
                else if (respuestaUsuario == "N")
                {
                    retroalimentacion = 0;
                    Console.WriteLine("\nNo te gustó la recomendación del servicio =(, pero mejoraremos.");
                }
                else
                {
                    Console.WriteLine("\nEl valor ingresado no es válido. Asumiremos que no te agradó la recomendación de comida.");
                }

                ServicioPersonalizer.EnviarRetroalimentacion(recomendacion, retroalimentacion);

                Console.WriteLine("\nPresiona Q para salir, o cualquier otra tecla para continuar:");
                continuar = !(LeerCaracter() == "Q");
            } while (continuar);
        }
    }
}