// using UnityEngine;
// using UnityEngine.TestTools;
// using NUnit.Framework;
// using System.Collections;

// public class PlayerCollisionTest
// {
//     private GameObject player;
//     private GameObject wall;

//     //[SetUp]
//     public void Setup()
//     {
//         // Crear el jugador
//         player = new GameObject("Player");
//         var rb = player.AddComponent<Rigidbody2D>();
//         rb.gravityScale = 0; // Sin gravedad para este test
//         rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Congelar rotación
//         var collider = player.AddComponent<BoxCollider2D>();

//         // Crear el script de movimiento del jugador
//         var playerMovement = player.AddComponent<PlayerMovement>();
//         playerMovement.speed = 5f;

//         // Crear una pared como objeto estático
//         wall = new GameObject("Wall");
//         wall.AddComponent<BoxCollider2D>();
//         wall.transform.position = new Vector3(2f, 0f, 0f); // Posicionar la pared frente al jugador
//     }

//     //[TearDown]
//     public void Teardown()
//     {
//         // Limpiar los objetos después del test
//         Object.Destroy(player);
//         Object.Destroy(wall);
//     }

//     //[UnityTest]
//     public IEnumerator PlayerStopsWhenCollidingWithWall()
//     {
//         // Configurar la posición inicial del jugador
//         player.transform.position = Vector3.zero;

//         // Simular entrada de movimiento hacia la derecha
//         InputManagerMock.SetHorizontalInput(1f);

//         // Esperar unos frames para que el jugador se mueva
//         yield return new WaitForSeconds(0.5f);

//         // Comprobar que el jugador se detuvo al chocar con la pared
//         Assert.LessOrEqual(player.transform.position.x, wall.transform.position.x, 
//             "El jugador atravesó la pared en lugar de detenerse.");
//     }
// }
