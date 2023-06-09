using Random = UnityEngine.Random;
using Mirror;

public class ItemDrop : NetworkBehaviour
{

public int RandomNumber;      // This is the variable, which is synchronized betweeen the host and a client 
                                   // It is generated every time HourGlass Coroutine starts(when every player press shoe icon showing, that they are ready to play)
                                   // Based on its value, if the players face a situation when the votes ae not equal (eg. 1-1-1)
                                   // It allows the game to randomize the choise based on random outcome of this number and take a random decision.
// This region describes how random numbers are generated to take decisions,
//  the chance of finding an item, and adding items to your inventory

  public int RandomNumberChanceToDropAnItem; // This random intiger is generated by every player after voting is finished. It determinates a CHANCE to find an item by each player

  private GameUI gameUI;

//////////////////////////////////////////////////////// RANDOM NUMBER GENERATOR ////////////////////////////////////////////////////////
public void NumberGenForDecision(){ 
{       
                RandomNumber = Random.Range(0,3);    // This method generates a random number, which is used in order to choose one of 3 options.
                                                     // It is synchronized via RPC with other players, so they will have
                                                     // the same number in their game and display the same option as every other player.

                if(isServer)                         // If we are the host, send the number directly via RPC
                {
                RpcSetRandomNumber(RandomNumber);             
                }
                if(!isServer)                        // If we are the client, use the command function first to avoid authority issues,
                                                     // Then in this CMD use the same RPC as above
                {
                CmdSetRandomNumber();
                }                              
}                                           
}
[Command(requiresAuthority = false)]       
private void CmdSetRandomNumber() {      
         RpcSetRandomNumber(RandomNumber);
}

[ClientRpc]                         
private void RpcSetRandomNumber (int NewRandomNumber) // Replace the old intiger with a new one and send this iformation to other players
{                                                       
    RandomNumber = NewRandomNumber;
}

//////////////////////////////////////////////////////// RANDOM CHANCE TO FIND AN ITEM ////////////////////////////////////////////////////////
public void NumberGenForItems(){                                                       // This random number is not set via RPC to everyone, otherwise everyone would have a panel
                                                                                        // with a drop. Not SynC this value means that only one person at the time finds an item and
                                                                                        // everyone generates a unique Random Number!(everyone )

                RandomNumberChanceToDropAnItem = Random.Range(0,3);                     // Set the number to be a random value from 0 to 2.
                RandomNumberGeneratorForItems(RandomNumberChanceToDropAnItem);          // Increase the range = items are not found that often
                //Debug.Log("Numer itemka to: " + RandomNumberChanceToDropAnItem);          
 }  

public void RandomNumberGeneratorForItems(int NewRandomNumberForItems)                  // Replace the old int, with a brand new generated one
{
    RandomNumberChanceToDropAnItem = NewRandomNumberForItems;
}
//////////////////////////////////////////////////////// ADD AN ITEM TO INVENTORY ////////////////////////////////////////////////////////

private void NotificationButtonClick()                       // When you press the "star" button(which is the symbol of a found item),
                                                            // You add your item to an inventory
{
      gameUI.NotificationAboutItem.SetActive(false);                // Once the star is pressed, it becomes invisible again
    if(isServer)                                            // If we are the host, we use RPC method place an item in other player's inventory
    {
        RpcAddRandomItem();
    }
    if(!isServer)
    {
        CmdAddRandomItem();                                 // If we are a client, we do the same, but firstly we use CMD to avoid authority issues.
    }
}
 [ClientRpc]
private void RpcAddRandomItem()
{
    GetComponent<AddItemToInventory>().AddItem();           // We take AddItemToInventory script(it has to be attatched on the same gameobject as this script to be visible)                                                           //
}                                                           // And from this cript we call AddItem method

[Command(requiresAuthority = false)]       
private void CmdAddRandomItem() {      
         RpcAddRandomItem();
    }


}
