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
            CreateOrUpdateEntity(message);
        }

        private void CreateOrUpdateEntity(Message message)
        {
            MovementScript movementScript;

            var position = new Vector3()
            {
                x = (float)message.Coordinates.X, z = (float)message.Coordinates.Y, y =
                    1.0f
            };


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