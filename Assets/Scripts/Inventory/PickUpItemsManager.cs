using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUpItemsManager : MonoBehaviour, ISaveable
{
    private static PickUpItemsManager instance;
    public List<PickUp> pickUps;

    [System.Serializable]
    public struct PickUpInfo
    {
        public string pickUpName;
        public bool picked_up;
        public int quantity;
        public Vector3 itemPosition;
        public Vector3 itemRotation;
        public Item pickupItem;
    }

    [System.Serializable]
    private struct SaveData
    {
        public List<PickUpInfo> pickUpInfos;
    }
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public static void AddPickUpToList(PickUp pickUp)
    {
        instance.pickUps.Add(pickUp);
    }

    public object SaveState()
    {
        //pickUps = FindObjectsOfType<PickUp>().ToList();
        pickUps.Sort((PickUp s1, PickUp s2) => s1.pickup_name.CompareTo(s2.pickup_name));
        SaveData save = new SaveData();
        List<PickUpInfo> info = new List<PickUpInfo>();
        foreach (PickUp pickUp in pickUps)
        {
            PickUpInfo newInfo;
            newInfo.pickUpName = pickUp.pickup_name;
            newInfo.picked_up = pickUp.picked_up;
            newInfo.itemPosition = pickUp.transform.position;
            newInfo.itemRotation = pickUp.transform.rotation.eulerAngles;
            newInfo.pickupItem = pickUp.item;
            newInfo.quantity = pickUp.quantity;
            info.Add(newInfo);
        }
        save.pickUpInfos = info;
        return save;
    }

    public void LoadState(object saveState)
    {
        string json = JsonConvert.SerializeObject(saveState);
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto
            //ReferenceResolverProvider = () => new GenericResolver<Item>(p => p.itemName)
        });
        List<PickUpInfo> info = saveData.pickUpInfos;

        for (int i = 0; i < info.Count; i++)
        {
            PickUp pickUp = pickUps.Find(x => x.item.itemID == info[i].pickupItem.itemID);
            if (pickUp != null)
            {
                pickUp.item = info[i].pickupItem;
                Debug.Log(pickUp.pickup_name + " " + info[i].pickUpName + " a" + pickUp.item.droppedItem);
                pickUp.picked_up = info[i].picked_up;
                pickUp.gameObject.SetActive(!pickUp.picked_up);
                if (pickUp.GetComponent<Rigidbody>())
                {
                    pickUp.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    pickUp.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
                pickUp.transform.position = info[i].itemPosition;
                pickUp.transform.rotation = Quaternion.Euler(info[i].itemRotation);
                pickUp.transform.position = info[i].itemPosition;
                pickUp.quantity = info[i].quantity;
            }
            else if (!info[i].picked_up)
            {
                Item foundItem = Resources.Load<Item>("ScriptableObjects/Items/" + info[i].pickupItem.itemType.ToString() + "/" + info[i].pickupItem.name);
                Debug.Log(foundItem.horizontalIcon.name + " " + foundItem.itemID);
                GameObject newPickUp = Instantiate(foundItem.droppedItem, info[i].itemPosition, Quaternion.Euler(info[i].itemRotation));
                newPickUp.GetComponent<PickUp>().quantity = info[i].quantity;
                // newPickUp.SetActive(!info[i].picked_up);
                if (newPickUp.GetComponent<Rigidbody>())
                {
                    newPickUp.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    newPickUp.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }
        }
        //picked_up = saveData.picked_up;
        //transform.position = saveData.itemPosition;
        //transform.rotation = Quaternion.Euler(saveData.itemRotation);
        //gameObject.SetActive(picked_up);
    }
}
