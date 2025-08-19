using System.Threading.Tasks;
using UnityEngine;

public class ImageResources : MonoBehaviour
{
    public Sprite[] ListItem;
    public Sprite[] ListMiniGameItem;
    public Sprite[] ListNewAreaImage;

    void Awake()
    {
        ListItem = Resources.LoadAll<Sprite>("Items");
        ListMiniGameItem = Resources.LoadAll<Sprite>("ListMiniGameItem");
        ListNewAreaImage = Resources.LoadAll<Sprite>("ListNewArea");
        
        // LoadImageAsync();
    }

    
    private async void LoadImageAsync()
    {
        await LoadSpriteFromResourcesAsync("Items");
        Debug.Log("Items loaded");
        await LoadSpriteFromResourcesAsync("ListMiniGameItem");
        Debug.Log("ListMiniGameItem loaded");
        await LoadSpriteFromResourcesAsync("ListNewArea");
        Debug.Log("ListNewArea loaded");
    }

    private async Task LoadSpriteFromResourcesAsync(string path)
    {
        ResourceRequest request = Resources.LoadAsync<Sprite>(path);
        
        while (!request.isDone)
        {
            Debug.Log("??");
            await Task.Yield();
        }
    }

    void TakeImagesFromResources(Sprite[] list, string path) => list = Resources.LoadAll<Sprite>(path); 

    public Sprite TakeImage(Sprite[] list, string name)
    {
        Sprite itemNeed = null;
        foreach (Sprite item in list){
            if(item.name.ToLower() == name.ToLower()) 
            {
                itemNeed = item;
                break;
            }
        }
        return itemNeed;
    }
}
