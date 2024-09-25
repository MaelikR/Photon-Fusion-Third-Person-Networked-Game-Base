using UnityEngine;
using Fusion;

public class LocalPlayerSetup : NetworkBehaviour
{
    [Header("Components to Disable for Remote Players")]
    public MonoBehaviour[] componentsToDisable; // Les composants spécifiques à désactiver
    public GameObject[] gameObjectsToDisable;   // Les GameObjects à désactiver

    void Start()
    {
        // Si l'objet n'est pas contrôlé par le joueur local
        if (!HasInputAuthority)
        {
            // Désactiver les composants spécifiés
            foreach (MonoBehaviour component in componentsToDisable)
            {
                if (component != null)
                {
                    component.enabled = false;
                }
            }

            // Désactiver les GameObjects spécifiés
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
            // Vous pouvez activer d'autres éléments spécifiques au joueur local ici si nécessaire
        }
    }
}
