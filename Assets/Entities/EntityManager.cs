using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Network;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class EntityManager : MonoBehaviour

    {
        public GameObject personPrefab;
        public GameObject vehiclePrefab;
        public GameObject stationaryPrefab;
        public LineRenderer lineRendererPrefab;
        private Dictionary<string, MovementScript> _entities = new Dictionary<string, MovementScript>();

        private NetworkController _networkController;


        private void Awake()
        {
            _networkController = FindObjectOfType<NetworkController>();
            _networkController.OnMessageReceived += HandleMessageReceived;
        }

        private void HandleMessageReceived(Message message)
        {
            if (message.ObjectType == "packet") CreatePacketArrow(message);
            else CreateOrUpdateEntity(message);
        }

        private void CreatePacketArrow(Message message)
        {
            var source = _entities.TryGetValue(message.SourceId, out MovementScript src);
            if (!source)
            {
                Debug.LogError("Source entity not found!");
            }


            var target = _entities.TryGetValue(message.TargetId, out MovementScript tar);

            if (!target)
            {
                Debug.LogError("Target entity not found!");
            }


            LineRenderer lineRenderer = Instantiate(lineRendererPrefab);
            lineRenderer.positionCount = 2;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.startWidth = 0.75f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            Vector3 sourcePos = src.transform.position;
            Vector3 targetPos = tar.transform.position;


            sourcePos.y += 1;

            targetPos.y += 1;


            lineRenderer.SetPosition(0, sourcePos);
            // Start a coroutine that makes the arrow grow from start to end
            StartCoroutine(GrowArrow(lineRenderer, sourcePos, targetPos, 2.5f, message));


            // Destroy the instantiated LineRenderer GameObject after 3 seconds
            Destroy(lineRenderer.gameObject, 3f);
        }

        IEnumerator GrowArrow(LineRenderer lineRenderer, Vector3 start, Vector3 end, float duration, Message message)
        {
            float randomRed = Random.Range(0f, 1f);
            float randomGreen = Random.Range(0f, 1f);
            float randomBlue = Random.Range(0f, 1f);

            // Create a random color using the generated values
            Color randomColor = new Color(randomRed, randomGreen, randomBlue);
            float progress = 0;
            while (progress < 1)
            {
                lineRenderer.startColor = randomColor;
                lineRenderer.endColor = randomColor;
                // if (_entities.Keys.First() == message.SourceId)
                // {
                //     lineRenderer.startColor = Color.blue;
                //     lineRenderer.endColor = Color.blue;
                // }
                //
                // if (_entities.Keys.First() == message.TargetId)
                // {
                //     lineRenderer.startColor = Color.red;
                //     lineRenderer.endColor = Color.red;
                // }


                Vector3 currentEnd = Vector3.Lerp(start, end, progress);
                lineRenderer.SetPosition(1, currentEnd);
                yield return null;
                progress += Time.deltaTime / duration;
            }

            lineRenderer.SetPosition(1, end);
        }


        private void CreateOrUpdateEntity(Message message)
        {
            MovementScript movementScript;

            var position = new Vector3()
            {
                x = (float)message.Coordinates.X, z = (float)message.Coordinates.Y, y =
                    1.0f
            };


            // Entities is already spawned and therefore requires only update to position
            if (_entities.TryGetValue(message.SourceId, out var ent))
            {
                movementScript = ent;
                movementScript.SetTargetPosition(position);
                movementScript.SetSpeed(1f);
            }
            else
            {
                GameObject prefab;
                switch (message.ObjectType)
                {
                    case "person":
                        prefab = personPrefab;
                        break;
                    case "vehicle":
                        prefab = vehiclePrefab;
                        break;
                    case "stationary":
                        prefab = stationaryPrefab;
                        break;
                    default:
                        Debug.LogError("Unknown ObjectType");
                        return;
                }

                GameObject newEntityObject = Instantiate(prefab, position, Quaternion.identity);
                movementScript = newEntityObject.GetComponent<MovementScript>();

                if (movementScript.objectText != null)
                {
                    movementScript.objectText.text = message.SourceId;
                }

                if (movementScript == null)
                {
                    Debug.LogError("Not Entity found on prefab");
                    return;
                }

                _entities.Add(message.SourceId, movementScript);
            }
        }
    }
}