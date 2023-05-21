using System.Collections.Generic;
using Network;
using UnityEngine;

namespace Mobility
{
    public class MobilityManager  : MonoBehaviour
    {
        private Dictionary<string,GameObject> _personObjects;
        private Dictionary<string, GameObject> _vehicleObjects;
        private GameObject _stationaryObject;
        private Message _message;

        public void ProcessNetworkRequest(Message message)
        {
            _message = message;
            var instruction = message.Instruction;

            switch (instruction)
            {
                case "createOrUpdatePerson":
                CreateOrUpdatePerson();
                    break;
                case "createOrUpdateVehicle":
                    CreateOrUpdateVehicle(); 
                    break;
                case "createStationary":
                    CreateStationary();
                    break;
            }
            
            

        }

        private void CreateOrUpdatePerson()
        {
            Debug.Log("CreateOrUpdatePersonCalled");
            var id = _message.Id;
            if (_personObjects.TryGetValue(id, out var o))
            {
                Debug.Log("Person already exists");

                o.transform.position = new Vector3(
                    (float)_message.Coordinates.X,
                    (float)_message.Coordinates.Y,
                    (float)_message.Coordinates.Z);
            }
            else
            {
                Debug.Log("Person doesn't exist already. Instantiating");

                //var newPerson = Instantiate(PersonPrefab);
               // newPerson.transform.position = new Vector3(
                //    (float)_message.Coordinates.X,
                //    (float)_message.Coordinates.Y,
               //    (float) _message.Coordinates.Z);
                //_personObjects.Add(id, newPerson);
            }

        }
        private void CreateOrUpdateVehicle()
        {
        }
        private void CreateStationary()
        {
            
        }

        

    }
}