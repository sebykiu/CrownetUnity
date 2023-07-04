using System;
using System.Collections.Generic;
using Entities;
using Network;
using UnityEngine;

namespace DefaultNamespace
{
    public class EntityManager : MonoBehaviour

    {
        public GameObject personPrefab;
        public GameObject vehiclePrefab;
        public GameObject stationaryPrefab;
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

            GameObject lineObject = new GameObject("PacketLine");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor =  new Color(0.5f, 0f, 0.5f, 1f);
            lineRenderer.endWidth = 0.5f;
            Vector3 sourcePos = src.transform.position;
            sourcePos.y += 1; // Replace '1' with the height of the model if you know it
            lineRenderer.SetPosition(0, sourcePos);

            Vector3 targetPos = tar.transform.position;
            targetPos.y += 1; // Replace '1' with the height of the model if you know it
            lineRenderer.SetPosition(1, targetPos);


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
                movementScript.objectText.text = message.SourceId;

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