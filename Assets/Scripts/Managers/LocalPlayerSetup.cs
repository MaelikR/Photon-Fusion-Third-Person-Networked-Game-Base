using UnityEngine;
using Fusion;

public class LocalPlayerSetup : NetworkBehaviour
{
    [Header("Components to Disable for Remote Players")]
    public MonoBehaviour[] componentsToDisable; // Les composants sp�cifiques � d�sactiver
    public GameObject[] gameObjectsToDisable;   // Les GameObjects � d�sactiver

    void Start()
    {
        // Si l'objet n'est pas contr�l� par le joueur local
        if (!HasInputAuthority)
        {
            // D�sactiver les composants sp�cifi�s
            foreach (MonoBehaviour component in componentsToDisable)
            {
                if (component != null)
                {
                    component.enabled = false;
                }
            }

            // D�sactiver les GameObjects sp�cifi�s
            foreach (GameObject obj in gameObjectsToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
        else
        {
            // Vous pouvez activer d'autres �l�ments sp�cifiques au joueur local ici si n�cessaire
        }
    }
}
