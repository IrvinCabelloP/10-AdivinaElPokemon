using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace _10_AdivinaElPokemon
{
    internal class Program
    {
        static readonly HttpClient client = new();

        public static async Task Main()
        {
            

            Console.WriteLine("Programa que simula el juego de adivina la palabra o ahorcado, pero con nombres de pokemones, esto lo hace conectadonce a la pokéapi y trayendo un nombre aleatoriamente.");
            int opcion;
            do
            {
                Console.WriteLine("¿Quieres jugar?");
                Console.WriteLine("\t1-Si");
                Console.WriteLine("\t2-No");
                opcion = Int32.Parse(Console.ReadLine());
                if (opcion == 2)
                {
                    break;
                }
                await ConexionPokeapi();
            } while (opcion != 2);
        }

        static int EleccionAleatoriaIDPokemon()
        {
            Random rnd = new();
            int random = rnd.Next(1, 1025);
            return random;
        }

        public static async Task ConexionPokeapi()
        {
            string url = "https://pokeapi.co/api/v2/pokemon/";
            try
            {
                int random = EleccionAleatoriaIDPokemon();
                string urlcompleta = url + (random.ToString());
                using (HttpResponseMessage response = await client.GetAsync(urlcompleta))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var pokemon = JsonSerializer.Deserialize<Pokemon>(responseBody);
                    if(pokemon != null)
                    {
                        string letrasOcultas = OcultarLetras(pokemon);
                        JuegoAdivinanza(pokemon, letrasOcultas);
                    }
                }                
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("¡Error encontrado! " + e.Message);
            }
        }

        public static string OcultarLetras(Pokemon pokemon)
        {            
            int cantidadCaracteres = pokemon.name.Length;
            int cantidadPalabrasOcultas = (int)Math.Ceiling(cantidadCaracteres * .40);
            char[] letrasOcultas = pokemon.name.ToCharArray();
            Random random = new();
            for(int i = 0; i <= cantidadPalabrasOcultas; i++)
            {
                int indice = random.Next(pokemon.name.Length);
                while (letrasOcultas[indice] == '_')
                {
                    indice = random.Next(pokemon.name.Length);
                }
                letrasOcultas[indice] = '_';
            }        
            return new string(letrasOcultas);
        }

        public static void JuegoAdivinanza(Pokemon pokemon, string letrasOcultas)
        {
            int maxintentos = 3;
            int intentos = 0;
            Console.WriteLine($"El pokémos a adivinar es: {letrasOcultas}");
            Console.WriteLine($"¿Una pista?, el Id del pokémon es {pokemon.id}\n");
            while(intentos != 3)
            {
                Console.WriteLine($"Tienes {maxintentos} intentos.");
                Console.WriteLine("¿Cuál es el pokémon?");
                string palabraUsuario = Console.ReadLine();
                if (palabraUsuario == pokemon.name)
                {
                    Console.WriteLine("¡Perfecto!, adivinaste el pokémon.\n");
                    break;
                }
                else
                {
                    Console.WriteLine("Respuesta equivocada. Vuelve a intentarlo.\n");
                    maxintentos =  maxintentos - 1;
                    intentos = intentos + 1;
                    if(maxintentos == 0)
                    {
                        Console.WriteLine($"¡Perdiste!, mejor suerte para la próxima, la respuesta correcta era: {pokemon.name}\n");
                        break;
                    }
                }
            }
        }        

        public class Pokemon
        {
            public int id { get; set;}
            public string name { get; set;}
        }
    }
}