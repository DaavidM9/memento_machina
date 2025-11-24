// using UnityEngine;
// using UnityEngine.TestTools;
// using NUnit.Framework;
// using System.Collections;

// public class PlayerMovementTest
// {
//     private GameObject player;
//     private PlayerMovement playerMovementScript;

//     //[SetUp]
//     public void Setup()
//     {
//         // Crea un objeto de prueba para el jugador
//         player = new GameObject("Player");
        
//         // Agrega un Rigidbody2D (para física) y un script de movimiento
//         var rb = player.AddComponent<Rigidbody2D>();
//         rb.gravityScale = 0; // Desactiva la gravedad para este test
        
//         playerMovementScript = player.AddComponent<PlayerMovement>();
//         playerMovementScript.speed = 5f; // Ajusta la velocidad del movimiento
//     }

//     //[TearDown]
//     public void Teardown()
//     {
//         // Limpia después de cada test
//         Object.Destroy(player);
//     }

//     //[UnityTest]
//     public IEnumerator PlayerMovesRightWhenKeyIsPressed()
//     {
//         // Simula una entrada de movimiento hacia la derecha
//         InputManagerMock.SetHorizontalInput(1f); // Mock de input (simula la tecla "D")
        
//         // Espera un frame para que el movimiento ocurra
//         yield return null;

//         // Verifica que el jugador se movió hacia la derecha
//         Assert.Greater(player.transform.position.x, 0, "El jugador no se movió hacia la derecha.");
//     }
// }

