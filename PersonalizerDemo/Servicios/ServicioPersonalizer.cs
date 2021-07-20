using System;
using System.Collections.Generic;

using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;

namespace PersonalizerDemo.Servicios
{
    public class ServicioPersonalizer
    {
        private static readonly string Llave = "";
        private static readonly string Endpoint = "";

        // Inicializa el cliente de Personalizer.
        private static PersonalizerClient clientePersonalizer = new PersonalizerClient(
            new ApiKeyServiceClientCredentials(Llave))
        {
            Endpoint = Endpoint
        };

        public static RankResponse ObtenerRecomendacion(IList<RankableAction> acciones, 
            List<object> contextoUsuario, 
            List<string> accionesIgnoradas)
        {
            // Genera un ID asociado a la petición.
            var idEvento = Guid.NewGuid().ToString();

            // realiza la petición al servicio
            var request = new RankRequest(acciones, contextoUsuario, accionesIgnoradas, idEvento);
            RankResponse respuesta = clientePersonalizer.Rank(request);

            return respuesta;
        }

        public static void EnviarRetroalimentacion(RankResponse recomendacion, float retroalimentacion)
        {
            clientePersonalizer.Reward(recomendacion.EventId, new RewardRequest(retroalimentacion));
        }
    }
}
