using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[AddComponentMenu("PoolManager/SpawnPool")]
public sealed class SpawnPool : MonoBehaviour, IList<Transform>
{
    #region Inspector Parameters

    /// Returns the name of this pool used by PoolManager. This will always be the
    /// same as the name in Unity, unless the name contains the work "Pool", which
    /// PoolManager will strip out. This is done so you can name a prefab or
    /// GameObject in a way that is development friendly. For example, "EnemiesPool" 
    /// is easier to understand than just "Enemies" when looking through a project.
    public string poolName = "";

    /// <summary>
    /// If true, this GameObject will be set to Unity's Object.DontDestroyOnLoad()
    /// </summary>
    public bool dontDestroyOnLoad = false;
    
    /// <summary>
    /// Print information to the Unity Console
    /// </summary>
    public bool logMessages = false;
	
	public Transform attachTo = null;

    /// <summary>
    /// A list of PreloadDef options objects used by the inspector for user input
    /// </summary>
    public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();
	//public PrefabPool[] _perPrefabPoolOptions;

    /// <summary>
    /// Used by the inspector to store this instances foldout states.
    /// </summary>
    public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();
    #endregion Inspector Parameters


    #region Public Code-only Parameters
    /// <summary>
    /// The time in seconds to stop waiting for particles to die.
    /// A warning will be logged if this is triggered.
    /// </summary>
    [HideInInspector]
    public float maxParticleDespawnTime = 60f;

    /// <summary>
    /// The group is an empty game object which will be the parent of all
    /// instances in the pool. This helps keep the scene easy to work with.
    /// </summary>
    public Transform group { get; private set; }

    /// <summary>
    /// Returns the prefab of the given name (dictionary key)
    /// </summary>
    public PrefabsDict prefabs = new PrefabsDict();

    // Keeps the state of each individual foldout item during the editor session
    public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();
    #endregion Public Code-only Parameters



    #region Private Properties
    private List<PrefabPool> _prefabPools = new List<PrefabPool>();
    private List<Transform> _spawned = new List<Transform>();
    #endregion Private Properties



    #region Constructor and Init
    private void Awake()
    {
        // Make this GameObject immortal if the user requests it.
        if (this.dontDestroyOnLoad) Object.DontDestroyOnLoad(this.gameObject);

        group = transform;

        // Default name behavior will use the GameObject's name without "Pool" (if found)
        if (this.poolName == "")
        {
            // Automatically Remove "Pool" from names to allow users to name prefabs in a 
            //   more development-friendly way. E.g. "EnemiesPool" becomes just "Enemies".
            //   Notes: This will return the original string if "Pool" isn't found.
            //          Do this once here, rather than a getter, to avoide string work
            this.poolName = this.group.name.Replace("Pool", "");
            this.poolName = this.poolName.Replace("(Clone)", "");
        }


        if (this.logMessages)
            Debug.Log(string.Format("SpawnPool {0}: Initializing..", this.poolName));

        // Only used on items defined in the Inspector
        foreach (PrefabPool prefabPool in this._perPrefabPoolOptions)
        {
            if (prefabPool.prefab == null)
            {
                //Debug.LogWarning(string.Format("Initialization Warning: Pool '{0}' "+
                //          "contains a PrefabPool with no prefab reference. Skipping.",
                //           this.poolName));
                continue;
            }

            // Init the PrefabPool's GameObject cache because it can't do it.
            //   This is only needed when created by the inspector because the constructor
            //   won't be run.
            prefabPool.inspectorInstanceConstructor();
            this.CreatePrefabPool(prefabPool);
        }

        // Add this SpawnPool to PoolManager for use. This is done last to lower the 
        //   possibility of adding a badly init pool.
        PoolManager.Pools.Add(this);
    }


    /// <summary>
    /// Runs when this group GameObject is destroyed and executes clean-up
    /// </summary>
    private void OnDestroy()
    {
        if (this.logMessages)
            Debug.Log(string.Format("SpawnPool {0}: Destroying...", this.poolName));

        PoolManager.Pools.Remove(this);

        this.StopAllCoroutines();

        // We don't need the references to spawns which are about to be destroyed
        this._spawned.Clear(); 

        // Clean-up
        foreach (PrefabPool pool in this._prefabPools) pool.SelfDestruct();

        // Probably overkill, and may not do anything at all, but...
        this._prefabPools.Clear();
        this.prefabs._Clear();
    }

    public void ClearCachGameObject(string fileName)
    {
        this.StopAllCoroutines();

        for (int i = 0; i < _spawned.Count; )
        {
            if (_spawned[i] != null && _spawned[i].name.Contains(fileName))
            {
                _spawned.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }

        foreach (PrefabPool pool in this._prefabPools)
        {
            if (pool.prefab.name.Contains(fileName))
            {
                prefabs._Remove(pool.prefab.name);
                pool.SelfDestruct();
                _prefabPools.Remove(pool);

                break;
            }
        }
    }


    /// <summary>
    /// Creates a new PrefabPool in this Pool and instances the requested 
    /// number of instances (set by PrefabPool.preloadAmount). If preload 
    /// amount is 0, nothing will be spawned and the return list will be empty.
    /// 
    /// It is rare this function is needed during regular usage.
    /// This function should only be used if you need to set the preferences
    /// of a new PrefabPool, such as culling or pre-loading, before use. Otherwise, 
    /// just use Spawn() and if the prefab is used for the first time a PrefabPool 
    /// will automatically be created with defaults anyway.
    /// 
    /// Note: Instances with ParticleEmitters can be preloaded too because 
    ///       it won't trigger the emmiter or the coroutine which waits for 
    ///       particles to die, which Spawn() does.
    ///       
    /// Usage Example:
    ///     // Creates a prefab pool and sets culling options but doesn't
    ///     //   need to spawn any instances (this is fine)
    ///     PrefabPool prefabPool = new PrefabPool()
    ///     prefabPool.prefab = myPrefabReference;
    ///     prefabPool.preloadAmount = 0;
    ///     prefabPool.cullDespawned = True;
    ///     prefabPool.cullAbove = 50;
    ///     prefabPool.cullDelay = 30;
    ///     
    ///     // Enemies is just an example. Any pool is fine.
    ///     PoolManager.Pools["Enemies"].CreatePrefabPool(prefabPool);
    ///     
    ///     // Then, just use as normal...
    ///     PoolManager.Pools["Enemies"].Spawn(myPrefabReference);
    /// </summary>
    /// <param name="prefabPool">A PrefabPool object</param>
    /// <returns>A List of instances spawned or an empty List</returns>
    public List<Transform> CreatePrefabPool(PrefabPool prefabPool)
    {
        // Only add a PrefabPool once. Uses a GameObject comparison on the prefabs
        //   This will rarely be needed and will almost Always run at game start, 
        //   even if user-executed. This really only fails If a user tries to create 
        //   a PrefabPool using a prefab which already has a PrefabPool in the same
        //   SpawnPool. Either user created twice or PoolManager went first or even 
        //   second in cases where a user-script beats out PoolManager's init during 
        //   Awake();
        bool isAlreadyPool = this.GetPrefab(prefabPool.prefab) == null ? false : true;
        if (!isAlreadyPool)
        {
            // Used internally to reference back to this spawnPool for things 
            //   like anchoring co-routines.
            prefabPool.spawnPool = this;

            this._prefabPools.Add(prefabPool);
            // Add to the prefabs dict for convenience
            this.prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
        }

        // Preloading (uses a singleton bool to be sure this is only done once)
        List<Transform> instances = new List<Transform>();
        if (prefabPool.preloaded != true)
        {
            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0}: Preloading {1} {2}",
                                           this.poolName,
                                           prefabPool.preloadAmount,
                                           prefabPool.prefab.name));

            // Preload and store result to the return list
            instances.AddRange(prefabPool.PreloadInstances());
        }

        return instances;
    }


    /// <summary>
    /// Add an existing instance to this pool. This is used during game start 
    /// to pool objects which are not instanciated at runtime
    /// </summary>
    /// <param name="instance">The instance to add</param>
    /// <param name="prefabName">
    /// The name of the prefab used to create this instance
    /// </param>
    /// <param name="despawn">True to depawn on start</param>
    /// <param name="parent">True to make a child of the pool's group</param>
    public void Add(Transform instance, string prefabName, bool despawn, bool parent)
    {
        foreach (PrefabPool prefabPool in this._prefabPools)
        {
            if (prefabPool.prefabGO == null)
            {
                Debug.LogError("Unexpected Error: PrefabPool.prefabGO is null");
                return;
            }

            if (prefabPool.prefabGO.name == prefabName)
            {
                prefabPool.AddUnpooled(instance, despawn);

                if (this.logMessages)
                    Debug.Log(string.Format(
                            "SpawnPool {0}: Adding previously unpooled instance {1}",
                                            this.poolName, 
                                            instance.name));

                //if (parent)
                //{
                //    instance.parent = this.group;
                //}

                AttachNew(this.group, false);

                // New instances are active and must be added to the internal list 
                if (!despawn) this._spawned.Add(instance);

                return;
            }
        }

        // Log an error if a PrefabPool with the given name was not found
        Debug.LogError(string.Format("SpawnPool {0}: PrefabPool {1} not found.",
                                     this.poolName,
                                     prefabName));

    }
    #endregion Constructor and Init



    #region List Overrides
    /// <summary>
    /// Not Implimented. Use Spawn() to properly add items to the pool.
    /// This is required because the prefab needs to be stored in the internal
    /// data structure in order for the pool to function properly. Items can
    /// only be added by instencing them using SpawnInstance().
    /// </summary>
    /// <param name="item"></param>
    public void Add(Transform item)
    {
        string msg = "Use SpawnPool.Spawn() to properly add items to the pool.";
        throw new System.NotImplementedException(msg);
    }


    /// <summary>
    /// Not Implimented. Use Despawn() to properly manage items that should remain 
    /// in the Queue but be deactivated. There is currently no way to safetly
    /// remove items from the pool permentantly. Destroying Objects would
    /// defeat the purpose of the pool.
    /// </summary>
    /// <param name="item"></param>
    public void Remove(Transform item)
    {
        string msg = "Use Despawn() to properly manage items that should " +
                     "remain in the pool but be deactivated.";
        throw new System.NotImplementedException(msg);
    }

    #endregion List Overrides



    #region Pool Functionality
	
	public void Attach(Transform parent, Transform child, bool beKeepLocal)
    {
		if (child.parent == parent)
		{
			return;
		}

        if (beKeepLocal)
        {
            Vector3 pos = child.localPosition;
            Quaternion rot = child.localRotation;
            Vector3 scale = child.localScale;
            child.parent = parent;
            child.localPosition = pos;
            child.localRotation = rot;
            child.localScale = scale;
        }
        else
        {
            child.parent = parent;
        }
    }

    void AttachNew(Transform inst, bool keepLocal)
	{
		if (attachTo)
		{
			Attach(attachTo, inst, keepLocal);
		}
        else
		{
			Attach(this.group, inst, keepLocal);			
		}
	}

    public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
    {
        return Spawn(prefab, pos, rot, false);
    }


    static void Activate(Transform t)
    {
        t.gameObject.SetActive(true);

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            Activate(child);
        }
    }

    static void Deactivate(Transform t)
    {
        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            Deactivate(child);
        }        

        t.gameObject.SetActive(false);
    }


    static public void SetActive(GameObject go, bool state)
    {
        if (state)
        {
            Activate(go.transform);
        }
        else
        {
            Deactivate(go.transform);
        }
    }

    public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, bool keepLocal)
    {
        Transform inst;
        
        foreach (PrefabPool prefabPool in this._prefabPools)
        {
            if (prefabPool.prefabGO == prefab.gameObject)
            {
                inst = prefabPool.SpawnInstance(pos, rot, keepLocal);

                if (inst == null)
                {
                    return null;
                }
                
                if (inst.gameObject.activeSelf == false)
                {
                    inst.gameObject.SetActive(true);                    
                }

				AttachNew(inst, keepLocal);

                if (inst != null)
                {
                    inst.gameObject.SendMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
                }

                this._spawned.Add(inst);
                return inst;
            }
        }        
        
        PrefabPool newPrefabPool = new PrefabPool(prefab);
        this.CreatePrefabPool(newPrefabPool);
        inst = newPrefabPool.SpawnInstance(pos, rot, keepLocal);
        
        if (inst.gameObject.activeSelf == false)
        {
            inst.gameObject.SetActive(true);            
        }

		AttachNew(inst, keepLocal);

        if (inst != null)
        {            
            inst.gameObject.SendMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
        }
        
        this._spawned.Add(inst);        
        return inst;
    }


    public Transform Spawn(Transform prefab)
    {
        return this.Spawn(prefab, Vector3.zero, Quaternion.identity, true);

    }

    public void Despawn(Transform xform)
    {        
        bool despawned = false;

        foreach (PrefabPool prefabPool in this._prefabPools)
        {
            if (prefabPool.spawned.Contains(xform))
            {
                despawned = prefabPool.DespawnInstance(xform);
                break;
            }  
            else if (prefabPool.despawned.Contains(xform))
            {
                //Debug.Log(
                //    string.Format("SpawnPool {0}: {1} has already been despawned. " +
                //                   "You cannot despawn something more than once!",
                //                    this.poolName,
                //                    xform.name));
                return;
            }
        }

       
        if (!despawned)
        {
            Debug.LogWarning(string.Format("SpawnPool {0}: {1} not found in SpawnPool",
                           this.poolName,
                           xform.name));
           return;
        }

        this._spawned.Remove(xform);
    }


    public void Despawn(Transform instance, float seconds)
    {
        this.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds));
    }

    private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.Despawn(instance);
    }


    public void DespawnAll()
    {
        var spawned = new List<Transform>(this._spawned);
        foreach (Transform instance in spawned)
            this.Despawn(instance);
    }

    public void ClearAll()
    {
        this.StopAllCoroutines();        
        this._spawned.Clear();        
        foreach (PrefabPool pool in this._prefabPools) pool.SelfDestruct();        
        this._prefabPools.Clear();
        this.prefabs._Clear();
    }


    public bool IsSpawned(Transform instance)
    {
        return this._spawned.Contains(instance);
    }

    #endregion Pool Functionality


    #region Utility Functions
    public Transform GetPrefab(Transform prefab)
    {
        foreach (PrefabPool prefabPool in this._prefabPools)
        {
            if (prefabPool.prefabGO == null)
                Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null",
                                             this.poolName));

            if (prefabPool.prefabGO == prefab.gameObject) 
                return prefabPool.prefab;
        }

        // Nothing found
        return null;
    }


    //assent bundle 删除后 要清除pool里面的缓存
    public void RemovePrefabPools(string prefabName)
    {
        foreach (PrefabPool prefabPool in this._prefabPools)
        {
            if (prefabPool.prefabGO.name.CompareTo(prefabName) == 0)
            {
                this.prefabs._Remove(prefabName);
                _prefabPools.Remove(prefabPool);
                break;
            }
        }
    }

    public GameObject GetPrefab(GameObject prefab)
    {
        foreach (PrefabPool prefabPool in this._prefabPools)
        {
            if (prefabPool.prefabGO == null)
                Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null",
                                             this.poolName));

            if (prefabPool.prefabGO == prefab)
                return prefabPool.prefabGO;
        }

        return null;
    }


    #endregion Utility Functions

    public Transform this[int index]
    {
        get { return this._spawned[index]; }
        set { throw new System.NotImplementedException("Read-only."); }
    }

    public bool Contains(Transform item)
    {
        string message = "Use IsSpawned(Transform instance) instead.";
        throw new System.NotImplementedException(message);
    }


    public void CopyTo(Transform[] array, int arrayIndex)
    {
        this._spawned.CopyTo(array, arrayIndex);
    }


    public int Count
    {
        get { return this._spawned.Count; }
    }

    public IEnumerator<Transform> GetEnumerator()
    {
        foreach (Transform instance in this._spawned)
            yield return instance;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (Transform instance in this._spawned)
            yield return instance;
    }

    // Not implemented
    public int IndexOf(Transform item) { throw new System.NotImplementedException(); }
    public void Insert(int index, Transform item) { throw new System.NotImplementedException(); }
    public void RemoveAt(int index) { throw new System.NotImplementedException(); }
    public void Clear() { throw new System.NotImplementedException(); }
    public bool IsReadOnly { get { throw new System.NotImplementedException(); } }
    bool ICollection<Transform>.Remove(Transform item) { throw new System.NotImplementedException(); }

}


[System.Serializable]
public class PrefabPool
{

    #region Public Properties Available in the Editor
    public Transform prefab;
    internal GameObject prefabGO;
    public int preloadAmount = 1;

    public bool limitInstances = false;
    public int limitAmount = 100;
    public bool cullDespawned = false;
    public int cullAbove = 10;
    public int cullDelay = 30;
    public int cullMaxPerPass = 5;
    public bool _logMessages = false;
    private bool logMessages
    {
        get 
        {
            if (forceLoggingSilent) return false;

            if (this.spawnPool.logMessages)
                return this.spawnPool.logMessages;
            else
                return this._logMessages; 
        }
    }


    private bool forceLoggingSilent = false;

    internal SpawnPool spawnPool;
    #endregion Public Properties Available in the Editor

    

    public PrefabPool(Transform prefab)
    {
        this.prefab = prefab;
        this.prefabGO = prefab.gameObject;
    }

    public PrefabPool() { }

    internal void inspectorInstanceConstructor()
    {
        this.prefabGO = this.prefab.gameObject;
        this.spawned = new List<Transform>();
        this.despawned = new List<Transform>();
    }

    public void SelfDestruct()
    {
        this.prefab = null;
        this.prefabGO = null;
        this.spawnPool = null;

        //Go through both lists and destroy everything
        foreach (Transform inst in this.despawned)
        {
            if (inst != null && inst.gameObject != null)
            {
                GameObject.Destroy(inst.gameObject);
            }
        }

        foreach (Transform inst in this.spawned)
        {
            if (inst != null && inst.gameObject.transform != null)
            {
                GameObject.Destroy(inst.gameObject);
            }
        }

        this.spawned.Clear();
        this.despawned.Clear();
    }

    #region Pool Functionality
    private bool cullingActive = false;
    internal List<Transform> spawned = new List<Transform>();
    internal List<Transform> despawned = new List<Transform>();

    internal int totalCount
    {
        get
        {
            int count = 0;
            count += this.spawned.Count;
            count += this.despawned.Count;
            return count;
        }
    }


    private bool _preloaded = false;
    internal bool preloaded
    {
        get { return this._preloaded; }
        private set { this._preloaded = value; }
    }

	
	public void Attach(Transform parent, Transform child)
    {
		if (child.parent == parent)
		{
			return;
		}
		
		Vector3 pos = child.localPosition;
		Quaternion rot = child.localRotation;
		Vector3 scale = child.localScale;
		child.parent = parent;
		child.localPosition = pos;
		child.localRotation = rot;
		child.localScale = scale;
    }


    internal bool DespawnInstance(Transform xform)
    {
        if (this.logMessages)
            Debug.Log(string.Format("SpawnPool {0} ({1}): Despawning '{2}'",
                                   this.spawnPool.poolName,
                                   this.prefab.name,
                                   xform.name));


        this.spawned.Remove(xform);
        this.despawned.Add(xform);
        xform.gameObject.SendMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);

        Attach(spawnPool.group, xform);
        Vector3 pos = xform.position;
        //xform.position = new Vector3(0,65536,0);
        pos.y = 65536;
        xform.position = pos;
        // xform.gameObject.SetActiveRecursively(false); 
        xform.gameObject.SetActive(false);

        if (!this.cullingActive && this.cullDespawned && this.totalCount > this.cullAbove)
        {
            this.cullingActive = true;
            this.spawnPool.StartCoroutine(CullDespawned());
        }
        return true;
    }


    internal IEnumerator CullDespawned()
    {
        if (this.logMessages)
            Debug.Log(string.Format("SpawnPool {0} ({1}): CULLING TRIGGERED! " +
                                      "Waiting {2}sec to begin checking for despawns...",
                                    this.spawnPool.poolName,
                                    this.prefab.name,
                                    this.cullDelay));


        yield return new WaitForSeconds(this.cullDelay);

        while (this.totalCount > this.cullAbove)
        {
            // Attempt to delete an amount == this.cullMaxPerPass
            for (int i = 0; i < this.cullMaxPerPass; i++)
            {
                // Break if this.cullMaxPerPass would go past this.cullAbove
                if (this.totalCount <= this.cullAbove)
                    break;  // The while loop will stop as well independently

                // Destroy the last item in the list
                if (this.despawned.Count > 0)
                {
                    Transform inst = this.despawned[0];
                    this.despawned.RemoveAt(0);
                    MonoBehaviour.Destroy(inst.gameObject);

                    if (this.logMessages)
                        Debug.Log(string.Format("SpawnPool {0} ({1}): " +
                                                "CULLING to {2} instances. Now at {3}.",
                                            this.spawnPool.poolName,
                                            this.prefab.name,
                                            this.cullAbove,
                                            this.totalCount));
                }
                else if (this.logMessages)
                {
                    Debug.Log(string.Format("SpawnPool {0} ({1}): " +
                                                "CULLING waiting for despawn. " + 
                                                "Checking again in {2}sec",
                                            this.spawnPool.poolName,
                                            this.prefab.name,
                                            this.cullDelay));

                    break;
                }
            }

            yield return new WaitForSeconds(this.cullDelay);
        }

        if (this.logMessages)
            Debug.Log(string.Format("SpawnPool {0} ({1}): CULLING FINISHED! Stopping",
                                    this.spawnPool.poolName,
                                    this.prefab.name));

        this.cullingActive = false;
        yield return null;
    }


    internal Transform SpawnInstance(Vector3 pos, Quaternion rot, bool keepLocal)
    {
        Transform inst;

        if (this.despawned.Count == 0)
        {            
            inst = this.SpawnNew(pos, rot, keepLocal);
        }
        else
        {
            inst = this.despawned[0];
            this.despawned.RemoveAt(0);
            this.spawned.Add(inst);

            if (inst == null)
            {
                var msg = "Make sure you didn't delete a despawned instance directly.";
                throw new MissingReferenceException(msg);
            }

            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0} ({1}): respawning '{2}'.",
                                        this.spawnPool.poolName, this.prefab.name, inst.name));
            if (keepLocal)
            {
                inst.localPosition = this.prefab.transform.localPosition;
                inst.localRotation = this.prefab.transform.localRotation;
                inst.localScale = this.prefab.transform.localScale;                
            }
            else
            {
                inst.localPosition = pos;
                inst.rotation = rot;                
            }
        }

        return inst;
    }

    internal Transform SpawnNew(Vector3 pos, Quaternion rot, bool keepLocal)
    {
        if (this.limitInstances && this.totalCount >= this.limitAmount)
        {
            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0} ({1}): " +
                                          "LIMIT REACHED! Not creating new instances!",
                                        this.spawnPool.poolName,
                                        this.prefab.name));
            return null;
        }

        Transform inst = null;

        if (keepLocal)
        {
            inst = (Transform)Object.Instantiate(this.prefab);
        }
        else
        {
            inst = (Transform)Object.Instantiate(this.prefab, pos, rot);
        }

        this.nameInstance(inst);
        Attach(this.spawnPool.group, inst);

        this.spawned.Add(inst);

        if (this.logMessages)
            Debug.Log(string.Format("SpawnPool {0} ({1}): Spawned new instance '{2}'.",
                                    this.spawnPool.poolName,
                                    this.prefab.name,
                                    inst.name));

        return inst;
    }


    internal void AddUnpooled(Transform inst, bool despawn)
    {
        this.nameInstance(inst);

        if (despawn)
        {            
            this.despawned.Add(inst);
        }
        else
        {
            this.spawned.Add(inst);
        }
    }


    internal List<Transform> PreloadInstances()
    {
        List<Transform> instances = new List<Transform>();

        if (this.preloaded)
        {
            Debug.Log(string.Format("SpawnPool {0} ({1}): " +
                                      "Already preloaded! You cannot preload twice. " +
                                      "If you are running this through code, make sure " +
                                      "it isn't also defined in the Inspector.",
                                    this.spawnPool.poolName,
                                    this.prefab.name));

            return instances;
        }

        if (this.prefab == null)
        {
            Debug.LogError(string.Format("SpawnPool {0} ({1}): Prefab cannot be null.",
                                         this.spawnPool.poolName,
                                         this.prefab.name));

            return instances;
        }

        // Reduce debug spam: Turn off this.logMessages then set it back when done.
        this.forceLoggingSilent = true;

        // Preload the amount requested
        // 	 Done by spawning and then immediatly de-spawning.
        Transform inst;
        while (this.totalCount < this.preloadAmount) // Total count will update
        {
            // This will parent, position and orient the instance under the SpawnPool.group
            inst = this.SpawnNew(Vector3.zero, Quaternion.identity, false);

            // This only happens if the limit option was used and reached
            // Print a warning because this only happens in PreloadInstances()
            //   if the user entered conflicting values
            //      preloadAmount > limitAmount
            if (inst == null)
            {
                Debug.LogError(string.Format("SpawnPool {0} ({1}): " +
                                    "You turned ON 'Limit Instances' and entered a " +
                                    "'Limit Amount' greater than the 'Preload Amount'!",
                                 this.spawnPool.poolName,
                                 this.prefab.name));
                continue;
            }

            inst.gameObject.SetActive(false);
            this.DespawnInstance(inst);

            instances.Add(inst); // The return list
        }

        // Restore the previous setting
        this.forceLoggingSilent = false;

        // Notify the user if they made a mistake using Culling
        //   (First check is cheap)
        if (this.cullDespawned && this.totalCount > this.cullAbove)
        {
            Debug.LogWarning(string.Format("SpawnPool {0} ({1}): " +
                        "You turned ON Culling and entered a 'Cull Above' threshold " +
                        "greater than the 'Preload Amount'! This will cause the " +
                        "culling feature to trigger immediatly, which is wrong " +
                        "conceptually. Only use culling for extreme situations. " +
                        "See the docs.",
                     this.spawnPool.poolName,
                     this.prefab.name));
        }

        return instances;
    }

    #endregion Pool Functionality



    #region Utilities
    /// <summary>
    /// Appends a number to the end of the passed transform. The number
    /// will be one more than the total objects in this PrefabPool, so 
    /// name the object BEFORE adding it to the spawn or depsawn lists.
    /// </summary>
    /// <param name="instance"></param>
    private void nameInstance(Transform instance)
    {
        // Rename by appending a number to make debugging easier
        //   ToString() used to pad the number to 3 digits. Hopefully
        //   no one has 1,000+ objects.
        instance.name += (this.totalCount + 1).ToString("#000");
    }


    #endregion Utilities

}





public class PrefabsDict : IDictionary<string, Transform>
{
    #region Public Custom Memebers

    /// Returns a formatted string showing all the prefab names
    public override string ToString()
    {
        // Get a string[] array of the keys for formatting with join()
        var keysArray = new string[this._prefabs.Count];
        this._prefabs.Keys.CopyTo(keysArray, 0);

        // Return a comma-sperated list inside square brackets (Pythonesque)
        return string.Format("[{0}]", System.String.Join(", ", keysArray));
    }
    #endregion Public Custom Memebers



    #region Internal Dict Functionality
    // Internal Add and Remove...
    internal void _Add(string prefabName, Transform prefab)
    {
        this._prefabs.Add(prefabName, prefab);
    }

    internal bool _Remove(string prefabName)
    {
        return this._prefabs.Remove(prefabName);
    }

    internal void _Clear()
    {
        this._prefabs.Clear();
    }
    #endregion Internal Dict Functionality


    #region Dict Functionality
    // Internal (wrapped) dictionary
    private Dictionary<string, Transform> _prefabs = new Dictionary<string, Transform>();

    /// Get the number of SpawnPools in PoolManager
    public int Count { get { return this._prefabs.Count; } }

    /// Returns true if a prefab exists with the passed prefab name.
    /// <param name="prefabName">The name to look for</param>
    /// <returns>True if the prefab exists, otherwise, false.</returns>
    public bool ContainsKey(string prefabName)
    {
        return this._prefabs.ContainsKey(prefabName);
    }

    /// Used to get a prefab when the user is not sure if the prefabName is used.
    /// This is faster than checking Contains(prefabName) and then accessing the dict
    public bool TryGetValue(string prefabName, out Transform prefab)
    {
        return this._prefabs.TryGetValue(prefabName, out prefab);
    }

    #region Not Implimented

    public void Add(string key, Transform value)
    { 
        throw new System.NotImplementedException("Read-Only"); 
    }

    public bool Remove(string prefabName) 
    { 
        throw new System.NotImplementedException("Read-Only"); 
    }

    public bool Contains(KeyValuePair<string, Transform> item)
    {
        string msg = "Use Contains(string prefabName) instead.";
        throw new System.NotImplementedException(msg);
    }

    public Transform this[string key]
    {
        get
        {
            Transform prefab;
            try
            {
                prefab = this._prefabs[key];
            }
            catch (KeyNotFoundException)
            {
                string msg = string.Format("A Prefab with the name '{0}' not found. " +
                                            "\nPrefabs={1}",
                                            key, this.ToString());
                throw new KeyNotFoundException(msg);
            }

            return prefab;
        }
        set
        {
            throw new System.NotImplementedException("Read-only.");
        }
    }

    public ICollection<string> Keys
    {
        get
        {
            string msg = "If you need this, please request it.";
            throw new System.NotImplementedException(msg);
        }
    }


    public ICollection<Transform> Values
    {
        get
        {
            string msg = "If you need this, please request it.";
            throw new System.NotImplementedException(msg);
        }
    }


    #region ICollection<KeyValuePair<string, Transform>> Members
    private bool IsReadOnly { get { return true; } }
    bool ICollection<KeyValuePair<string, Transform>>.IsReadOnly { get { return true; } }

    public void Add(KeyValuePair<string, Transform> item)
    {
        throw new System.NotImplementedException("Read-only");
    }

    public void Clear() { throw new System.NotImplementedException(); }

    private void CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
    {
        string msg = "Cannot be copied";
        throw new System.NotImplementedException(msg);
    }

    void ICollection<KeyValuePair<string, Transform>>.CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
    {
        string msg = "Cannot be copied";
        throw new System.NotImplementedException(msg);
    }

    public bool Remove(KeyValuePair<string, Transform> item)
    {
        throw new System.NotImplementedException("Read-only");
    }
    #endregion ICollection<KeyValuePair<string, Transform>> Members
    #endregion Not Implimented




    #region IEnumerable<KeyValuePair<string, Transform>> Members
    public IEnumerator<KeyValuePair<string, Transform>> GetEnumerator()
    {
        return this._prefabs.GetEnumerator();
    }
    #endregion



    #region IEnumerable Members
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this._prefabs.GetEnumerator();
    }
    #endregion

    #endregion Dict Functionality

}
