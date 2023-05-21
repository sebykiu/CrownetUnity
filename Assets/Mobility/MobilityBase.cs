using System.Collections.Generic;
using UnityEngine;

namespace Mobility
{
    public class MobilityBase
    {
        private List<GameObject> _personObjects;
        private List<GameObject> _vehicleObjects;
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
            
        }
        private void CreateOrUpdateVehicle()
        {
        }
        private void CreateStationary()
        {
            
        }

        

    }
}