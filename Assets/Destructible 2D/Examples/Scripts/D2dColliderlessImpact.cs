using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dColliderlessImpact))]
	public class D2dColliderlessImpact_Editor : D2dEditor<D2dColliderlessImpact>
	{
		protected override void OnInspector()
		{
			DrawDefault("ImpactPrefab");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component will raycast all destructibles between the previous and current position of this GameObject
	// If a solid pixel is found, then this GameObject will be destroyed, and the impact prefab will be spawned in its place
	[ExecuteInEditMode]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Colliderless Impact")]
	public class D2dColliderlessImpact : MonoBehaviour
	{
		[Tooltip("The prefab that gets spawned once this GameObject hits a destructible")]
		public GameObject ImpactPrefab;

		[SerializeField]
		private Vector3 oldPosition;

		protected virtual void OnEnable()
		{
			oldPosition = transform.position;
		}

		protected virtual void Start()
		{
			oldPosition = transform.position;
		}

		protected virtual void FixedUpdate()
		{
			var newPosition = transform.position;
			var hit         = D2dDestructible.RaycastAlphaFirst(oldPosition, newPosition);

			// Hit something?
			if (hit != null)
			{
				// Spawn a prefab at the impact point?
				if (ImpactPrefab != null)
				{

                    Vector2 direction;
                    Vector2 origin;
                    //int destLayerMask = (int)0xFFFFFFFF;

                    origin.x = oldPosition.x;
                    origin.y = oldPosition.y;
                    direction.x = (newPosition - oldPosition).x;
                    direction.y = (newPosition - oldPosition).y;


                    // Perform raycast in the direction of the projectile
                    var physicsHit = Physics2D.Raycast(origin, direction, 12.0f);
                    //Debug.Log("Raycast from " + origin + "to direction: " + direction);
                    if (physicsHit.collider != null)
                    {
                        // Get normal of the raycast
                        Vector2 normal = physicsHit.normal;
                        Debug.Log("Raycast hit at normal direction:" + normal + "; Hit location: " + physicsHit.point);
                        //Debug.Log("Raycast hit collider with center:" + physicsHit.collider.bounds.center);
                        //Debug.DrawRay(new Vector3(physicsHit.point.x, physicsHit.point.y, 0), new Vector3(normal.x, normal.y, 0), Color.red, 100.0f, false);
                        GameObject newObject = Instantiate(ImpactPrefab, hit.Position, transform.rotation) as GameObject;
                        D2dExplosion explosion = newObject.GetComponent<D2dExplosion>();
                        if (explosion != null)
                        {
                            if (explosion.StampNormal)
                            {
                                //Debug.Log("Explosion found and angle changed.");
                                explosion.StampAngle = Mathf.Atan2(normal.y, normal.x) / Mathf.PI / 2.0f * 360.0f + 90.0f;
                                
                            }

                            // GameObject of the collider hit
                            GameObject hitObject = physicsHit.collider.gameObject;
                            GameObject hitParent = hitObject.transform.root.gameObject;
                            if (hitObject != null)
                            {

                                HardnessProperty properties = hitParent.GetComponent<HardnessProperty>();
                                D2dDestructible destructibleComponent = hitParent.GetComponent<D2dDestructible>();
                                if (properties && destructibleComponent)
                                {
 
                                    // If hit object is a destructible object and has Hardness Properties
                                    if (properties.UseHardness && properties.Hardness <= 0.0f)
                                    {
                                        destructibleComponent.Indestructible = true;
                                    }
                                    else if (properties.UseHardness)
                                    {

                                        explosion.StampSize = new Vector2(1.0f/properties.Hardness, 1.0f/properties.Hardness);
                                    }
                                    else if (!properties.UseHardness)
                                    {
                                        // Do nothing
                                    }
                                }
                            }

                            explosion.triggerExplosion();
                        }

                    }


                }

				// Destroy this GameObject
				Destroy(gameObject);
			}

			oldPosition = newPosition;
		}
	}
}
