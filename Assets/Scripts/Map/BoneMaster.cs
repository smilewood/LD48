using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class BoneMaster : MonoBehaviour
{
    static List<GameObject> HBones;
    static List<GameObject> VBones;
    static GameObject _mapGO;

    static float _boneSideSize;

    public float BoneSideSize;

    public GameObject MapGameObject;
    public GameObject StartingBone;

    private HashSet<Vector2> ActiveBones;

    void Awake()
    {
        HBones = Resources.LoadAll("Prefabs/Bones/Horizontal", typeof(GameObject)).Select(go => go as GameObject).ToList();
        VBones = Resources.LoadAll("Prefabs/Bones/Vertical", typeof(GameObject)).Select(go => go as GameObject).ToList();
        _mapGO = MapGameObject;
        _boneSideSize = BoneSideSize;
        ActiveBones = new HashSet<Vector2>();

        map = new Dictionary<Vector2, Bone>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Bone temp = Bone.CreateHorizBone(new Vector2(0, 0));
        //temp.InitMissingNeighbors();
        //foreach (Vector2 neighbor in temp.neighbors)
        //{
        //    map[neighbor].InitMissingNeighbors();
        //}
        Bone.InitFirstBone(StartingBone);
        ActiveBones.Add(Vector2.zero);
        ActiveBones.UnionWith(map[Vector2.zero].neighbors);


        foreach (Bone b in map.Values)
        {
            b.floorTilemap.RefreshAllTiles();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckUpdatedPlayerPosition(Vector2 pos, GameObject player)
    {
        foreach (Bone bone in map.Values)
        {
            if (bone.PointInBone(pos))
            {
                int frameoffset = 0;
                bone.InitMissingNeighbors();
                bone.GameObject.GetComponent<BoneItializer>()?.OnPlayerInBone(player);

                //enable and disable behavior in other bones as well
                foreach (Vector2 active in ActiveBones.Where(b => !(bone.neighbors.Contains(b) || b == bone.origin)).ToList())
                {
                    //disable
                    map[active].GameObject.SetActive(false);
                    ActiveBones.Remove(active);
                }
                foreach (Vector2 active in bone.neighbors)
                {
                    map[active].GameObject.SetActive(true);
                    ActiveBones.Add(active);
                    StartCoroutine(RefreshTileEdges(map[active], frameoffset+=2));
                    //map[active].RefreshEdgeTiles();
                }
                //bone.RefreshEdgeTiles();
                StartCoroutine(RefreshTileEdges(bone, frameoffset+=2));
                break;
            }
        }
    }

    private IEnumerator RefreshTileEdges(Bone bone, int frame)
    {
        for(int i = 0; i < frame; ++i)
        {
            yield return new WaitForEndOfFrame();
        }
        bone.RefreshEdgeTiles();
        yield return null;
    }


    #region data structure

    static Dictionary<Vector2, Bone> map;

    public static Bone[] GetNeighboringBones(Vector2 pos)
    {
        if (map.ContainsKey(pos))
        {
            return map[pos].neighbors.Where(neighbor => map.ContainsKey(neighbor)).Select(pos => map[pos]).ToArray();
        }
        else
        {
            bool temp = true;
        }
        return null;
    }

    public static Bone GetBoneAtPos(Vector2 pos)
    {
        if (map.ContainsKey(pos))
        {
            return map[pos];
        }
        return null;
    }

    

    public abstract class Bone
    {
        public List<Vector2> neighbors;
        public GameObject GameObject;
        public Tilemap floorTilemap;

        public Vector2 origin;

        public bool neighborsInitialized = false;

        public abstract void InitMissingNeighbors();

        protected abstract void Init(Vector2 pos);

        public abstract bool PointInBone(Vector2 point);

        public abstract void RefreshEdgeTiles();


        protected void RefreshTiles(int right, int bottom)
        {
            //top, bottom
            for(int i = 0; i <= right; ++i)
            {
                floorTilemap.RefreshTile(new Vector3Int(i, -1, 0));
                floorTilemap.RefreshTile(new Vector3Int(i, bottom, 0));
            }
            //left, right
            for (int i = 0; i >= bottom; --i)
            {
                floorTilemap.RefreshTile(new Vector3Int(0, i, 0));
                floorTilemap.RefreshTile(new Vector3Int(right, i, 0));
            }
        }

        public static void InitFirstBone(GameObject prefab)
        {
            HorizBone bone = new HorizBone();
            bone.Init(Vector2.zero);
            map.Add(Vector2.zero, bone);

            GameObject boneGO = Instantiate(prefab, Vector3.zero, Quaternion.identity, _mapGO.transform);
            boneGO.name = "Start (0, 0)";
            boneGO.GetComponent<BoneItializer>()?.OnBoneCreate(Vector2.zero, false);
            bone.GameObject = boneGO;
            bone.floorTilemap = GetBoneTilemap(bone);
            bone.InitMissingNeighbors();
        }

        protected static VertBone CreateVertBone(Vector2 pos)
        {
            VertBone bone = new VertBone();
            
            bone.Init(pos);
            map.Add(pos, bone);

            //Add tiles to world
            GameObject newBone = Instantiate(VBones[Random.Range(0, VBones.Count)], new Vector3(pos.x * _boneSideSize, pos.y * _boneSideSize, 0), Quaternion.identity, _mapGO.transform);
            newBone.name = string.Format("BoneV ({0}, {1})", pos.x, pos.y);
            newBone.GetComponent<BoneItializer>()?.OnBoneCreate(pos, true);
            bone.GameObject = newBone;

            bone.floorTilemap = GetBoneTilemap(bone);
            

            return bone;
        }

        public static HorizBone CreateHorizBone(Vector2 pos)
        {
            HorizBone bone = new HorizBone();

            bone.Init(pos);
            map.Add(pos, bone);

            //Add tiles to world
            GameObject newBone = Instantiate(HBones[Random.Range(0, HBones.Count)], new Vector3(pos.x * _boneSideSize, pos.y * _boneSideSize, 0), Quaternion.identity, _mapGO.transform);
            newBone.name = string.Format("BoneH ({0}, {1})", pos.x, pos.y);
            newBone.GetComponent<BoneItializer>()?.OnBoneCreate(pos, false);
            bone.GameObject = newBone;


            bone.floorTilemap = GetBoneTilemap(bone);
            return bone;
        }
        private static Tilemap GetBoneTilemap(Bone boneToUbdate)
        {
            foreach (Transform child in boneToUbdate.GameObject.transform)
            {
                if (child.gameObject.CompareTag("floor"))
                {
                    return child.gameObject.GetComponent<Tilemap>();
                }
            }
            return null;
        }
    }

    public class VertBone : Bone
    {
        public override bool PointInBone(Vector2 point)
        {
            float minX = this.origin.x * _boneSideSize;
            float maxX = (this.origin.x * _boneSideSize) + (_boneSideSize);

            float minY = (this.origin.y * _boneSideSize) - (_boneSideSize * 2);
            float maxY = this.origin.y * _boneSideSize;

            return (minX <= point.x && point.x < maxX) && (minY <= point.y && point.y < maxY);
        }

        protected override void Init(Vector2 pos)
        {
            this.origin = pos;
            this.neighbors = new List<Vector2>()
            {
               new Vector2(pos.x, pos.y+1),
               new Vector2(pos.x+1, pos.y),
               new Vector2(pos.x+1, pos.y-1),
               new Vector2(pos.x-1, pos.y-2),
               new Vector2(pos.x-2, pos.y-1),
               new Vector2(pos.x-1, pos.y+1)
            };
        }

        public override void InitMissingNeighbors()
        {
            if (!map.ContainsKey(neighbors[0]))
            {
                CreateHorizBone(neighbors[0]);
            }
            if (!map.ContainsKey(neighbors[1]))
            {
                CreateHorizBone(neighbors[1]);
            }
            if (!map.ContainsKey(neighbors[2]))
            {
                CreateVertBone(neighbors[2]);
            }
            if (!map.ContainsKey(neighbors[3]))
            {
                CreateHorizBone(neighbors[3]);
            }
            if (!map.ContainsKey(neighbors[4]))
            {
                CreateHorizBone(neighbors[4]);
            }
            if (!map.ContainsKey(neighbors[5]))
            {
                CreateVertBone(neighbors[5]);
            }
            neighborsInitialized = true;
            
        }

        public override void RefreshEdgeTiles()
        {
            RefreshTiles(31, -64);
        }
    }

    public class HorizBone : Bone
    {
        public override bool PointInBone(Vector2 point)
        {
            float minX = this.origin.x * _boneSideSize;
            float maxX = (this.origin.x * _boneSideSize) + (_boneSideSize * 2);

            float minY = (this.origin.y * _boneSideSize) - _boneSideSize;
            float maxY = this.origin.y * _boneSideSize;

            return (minX <= point.x && point.x < maxX) && (minY <= point.y && point.y < maxY);
        }

        protected override void Init(Vector2 pos)
        {
            this.origin = pos;
            this.neighbors = new List<Vector2>()
            {
                new Vector2(pos.x-1, pos.y+1),
                new Vector2(pos.x+1, pos.y+2),
                new Vector2(pos.x+2, pos.y+1),
                new Vector2(pos.x+1, pos.y-1),
                new Vector2(pos.x, pos.y-1),
                new Vector2(pos.x-1, pos.y)
            };
        }

        public override void InitMissingNeighbors()
        {
            if (!map.ContainsKey(neighbors[0]))
            {
                CreateHorizBone(neighbors[0]);
            }
            if (!map.ContainsKey(neighbors[1]))
            {
                CreateVertBone(neighbors[1]);
            }
            if (!map.ContainsKey(neighbors[2]))
            {
                CreateVertBone(neighbors[2]);
            }
            if (!map.ContainsKey(neighbors[3]))
            {
                CreateHorizBone(neighbors[3]);
            }
            if (!map.ContainsKey(neighbors[4]))
            {
                CreateVertBone(neighbors[4]);
            }
            if (!map.ContainsKey(neighbors[5]))
            {
                CreateVertBone(neighbors[5]);
            }
            neighborsInitialized = true;
        }

        public override void RefreshEdgeTiles()
        {
            RefreshTiles(63, -32);
        }
    }



    #endregion


}
