// using UnityEngine;
// using UnityEngine.TestTools;
// using NUnit.Framework;
// using System.Collections;

// public class AudioTest
// {
//     private GameObject player;
//     private AudioSource soundEffectSource;
//     private AudioSource musicSource;

//    // [SetUp]
//     public void Setup()
//     {
//         // Crear el jugador
//         player = new GameObject("Player");

//         // Agregar un componente de audio para efectos de sonido
//         soundEffectSource = player.AddComponent<AudioSource>();
//         soundEffectSource.playOnAwake = false; // No reproducir al inicio

//         // Crear un objeto para la música de fondo
//         var musicObject = new GameObject("BackgroundMusic");
//         musicSource = musicObject.AddComponent<AudioSource>();
//         musicSource.playOnAwake = true; // Reproducir automáticamente al inicio
//         musicSource.loop = true; // Música en bucle
//     }

//    // [TearDown]
//     public void Teardown()
//     {
//         // Limpiar los objetos después del test
//         Object.Destroy(player);
//         Object.Destroy(soundEffectSource.gameObject);
//         Object.Destroy(musicSource.gameObject);
//     }

//     //[UnityTest]
//     public IEnumerator BackgroundMusicPlaysAtStart()
//     {
//         // Esperar un frame para asegurarnos de que la música comience
//         yield return null;

//         // Verificar que la música está reproduciéndose
//         Assert.IsTrue(musicSource.isPlaying, "La música de fondo no se está reproduciendo al inicio del juego.");
//     }

//    // [UnityTest]
//     public IEnumerator SoundEffectPlaysOnEvent()
//     {
//         // Simular un evento que activa el sonido (ejemplo: un salto)
//         soundEffectSource.Play();

//         // Esperar un frame para verificar el estado del audio
//         yield return null;

//         // Verificar que el efecto de sonido está reproduciéndose
//         Assert.IsTrue(soundEffectSource.isPlaying, "El efecto de sonido no se reprodujo al activarse el evento.");
//     }
// }
