using System.Collections.Generic;
using UnityEngine;
using RFG.Items;

namespace RFG.Platformer
{
  [CreateAssetMenu(fileName = "New Add Movement State Consumable", menuName = "RFG/Platformer/Items/Consumable/Add Movement State")]
  public class AddMovementStateConsumable : Consumable
  {
    public List<State> AbilityStates;

    public override void Consume(Transform transform, Inventory inventory)
    {
      base.Consume(transform, inventory);
      Character character = transform.gameObject.GetComponent<Character>();
      if (character != null)
      {
        AbilityStates.ForEach(state => character.MovementState.Add(state));
      }
    }
  }
}