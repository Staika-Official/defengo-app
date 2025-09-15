using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using UnityEngine.AddressableAssets;
using Framework.Util;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework.Sound;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Framework.Game.Defense
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance;
        public bool isGameOver = false;
        public int width = 22;
        public int height = 20;

        //그리드의 셀 데이터 들을 제네레이팅 하고 저장하기위한 데이터
        //SetGrid에서 제네레이팅 해줌
        public List<CellData> cellDatas = new();
        public GameObject backGround;
        public List<int> monsterPath;

        public UnityAction damageAction;

        public delegate void OnComplete_GenerateGrid(int[] value);
        public static OnComplete_GenerateGrid onComplete_GenerateGrid;
        public List<Glacier> glaciersTiles = new();

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            //데미지를 입으면 호출해줄 콜백함수 NormalDamageAction
            damageAction = NormalDamageAction;

            monsterPath.Capacity = 70;

            //그리드 제네레이팅 해주는 곳
            SetGrid();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Damage();
            }
        }

        public List<int> ConstructibleTiles()
        {
            //안 깨져있는 빙하 인덱스를 담을 리스트 선언
            // 이유 : 인덱스를 이용해서 가볍게 값을 넘겨주기 위함
            List<int> data = new();

            //빙하 타일만큼 순회한다
            for (int i = 0; i < glaciersTiles.Count; i++)
            {
                //빙하 타일의 스테이트가 안깨진 빙하면 사라진 빙하를 넘겨줄 값에 추가해준다
                if (glaciersTiles[i].glacierState != GlacierState.DESTROYED)
                {
                    data.Add(glaciersTiles[i].glacierIndex);
                }
            }

            return data;
        }

        public async void GetStageData()
        {
            //받아온 제이슨 파일을 가져와서 타일 데이터들을 받아온다
            StageData stageDatas = await DataLoadManager.Instance.GetDataAsyncBinary<StageData>("StageData");

            //빙하 타일을 프리팹을 얻어온다
            GameObject glacierTile = await DataLoadManager.Instance.GetDataAsync<GameObject>("GlacierTile");
            //적 이동 타일 프리팹을 얻어온다
            GameObject enemyTile = await DataLoadManager.Instance.GetDataAsync<GameObject>("EnemyTile");

            //받아온 타일 데이터들을 순회하며 데이터들을 받아오며 
            for (int i = 0; i < stageDatas.tileDatas.Length; i++)
            {
                TileData data = stageDatas.tileDatas[i];

                switch (data.landType)
                {
                    //랜드 타입이 1번일경우 (캐릭터 생성가능 위치) 빙하 오브젝트 셋팅
                    case 1:
                        SetConstructibleGrid(data, glacierTile);
                        break;
                    //몬스터 생성가능 위치
                    case 4:
                        SetMonsterPathGrid(data);
                        break;
                }
            }

            //빙하의 오더레이어를 설정해줌으로 캐릭터를 스폰할 때 캐릭터마다 레이어설정을해주기위해 함수 호출
            SetSortingValue();

            //만약 제네레이팅이 됬다면 저장해둔 몬스터가 지나다니는 길을 몬스터 인포에 넘겨준다

            onComplete_GenerateGrid?.Invoke(GetMonsterPath());
        }

        public void SetSortingValue()
        {
            int basicLayer = 10;
            int prevIndex = 0;

            //빙하 타일들의 갯수만큼 순회한다
            for (int i = 0; i < glaciersTiles.Count; i++)
            {
                //만약 현재 바운더리 가운데 인덱스가 이전 인덱스보다 크다면
                if (glaciersTiles[i].boundaryIndexes[4] > prevIndex)
                {
                    //빙하 타일의 바운더리 인덱스 설정을해서 이전 인덱스로 표
                    prevIndex = glaciersTiles[i].boundaryIndexes[4];

                    //빙하 타일의 오더 레이어를 설정해줌
                    glaciersTiles[i].orderLayer = basicLayer;

                    //일반 레이어 값 올려줌
                    basicLayer++;
                }
                //만약 현재 바운더리 가운데 인덱스가 이전 인덱스보다 작다면
                else
                {
                    //현재바운더리 인덱스값을 셋팅
                    prevIndex = glaciersTiles[i].boundaryIndexes[4];
                    //디폴트 레이어 값으로 셋팅
                    basicLayer = 10;
                    //디폴트 레이어 값으로 변경
                    glaciersTiles[i].orderLayer = basicLayer;

                    //행이 바뀌면 디폴트 오더레이어를 초기화해준다
                    //의도 : 설정해준 빙하 그리드의 오더레이어로 캐릭터를 스폰했을 때 캐릭터의 렌더 순서를 결정해주기위해
                    //행 바뀌는 부분에서 의도대로 흘러가지않음 예 5번 빙하와 6번 빙하의 오더레이어가 같음
                }
            }
        }

        public void PostRelocationProcess()
        {
            //호출 순서 : Relocation 버튼클릭 -> Relocation함수 호출 -> 
            //RelocationCharacter 함수 호출  -> PostRelocationProcess 함수 불림


            //빙하 카운트를 확인하면서 깨진 빙하인지 확인해서
            //깨진 빙하면 불가능한 소환으로 bool변수를 바꿔 줌 
            for (int i = 0; i < glaciersTiles.Count; i++)
            {
                if (glaciersTiles[i].glacierState == GlacierState.DESTROYED)
                {
                    glaciersTiles[i].isImpossibleSummon = true;
                }
            }
        }

        //몬스터의 이동 그리드를 가지고있는 배열을 넘겨주는함수
        public int[] GetMonsterPath()
        {
            int[] temp = monsterPath.ToArray();

            return temp;
        }

        //몬스터의 이동그리드의 데이터를 얻어오기 위한 함수
        public CellData GetCellData(int idx)
        {
            int tileIdx = monsterPath[idx];

            CellData data = cellDatas[tileIdx];
            return data;
        }

        public void SetConstructibleGrid(TileData data, GameObject constructibleTile)
        {
            //TileData 값
            //타일 인덱스 -> 포지션을 얻어올 수 있다.
            //랜드 타입 -> 해당 지역이 몬스터가 지나갈수있는자리냐 아니냐, 빙하친구들이 올라올 수 있는 자리냐 아니냐
            //스타팅 포인트??.. -> 빙하 그리드 시작위치
            //에셋네임 -> 어드레서블에서 값을 가져올때 사용하는 값 같음
            //가로 -> 가로크TileData
            //세로 -> 세로크기

            //빙하 그리드 시작위치 설정
            int column = data.startingPoint;

            //타일 프리팹 생성 
            GameObject obj = Instantiate(constructibleTile);

            //타일 프리팹에있는 글레시아 컴포넌트 가져온다
            Glacier glacier = obj.GetComponent<Glacier>();

            //데이터 행 -> 열 순으로 순회
            for (int i = 0; i < data.height; i++)
            {
                //행의 열 스타트 포인트 정하기
                int row = column;
                for (int j = 0; j < data.width; j++)
                {
                    //빙하 오브젝트가 포함하고있는 그리드 인덱스를 저장해주기 위함
                    //기획서 1 참고
                    glacier.boundaryIndexes.Add(row);

                    //셀 데이터 열 값의 타입을 정해준다
                    //CellType
                    // public enum CellType
                    //NONE, -> 아무것도없다
                    //CONSTRUCTIBLE, -> 캐릭터 생성 가능 타입?
                    //MOVABLE, -> 이동가능자리 (아마 몬스터 이동 가능 타입인지 체크인듯)
                    //BROKEN -> 부서진 빙하 타입

                    //랜드 타입으로 이미 정해진 건설가능 셀이기에 셀타입에 생성가능 타입을 지정해줌
                    //현재 그리드의 타입을 생성가능 그리드로 설정해주기위한 작업
                    cellDatas[row].cellType = CellType.CONSTRUCTIBLE;

                    //바운더리 인덱스를 지정해주기위한 작업
                    //빙하 그리드를 보면 열은 1씩 증가하기에 1씩증가시켜줌
                    row++;
                }

                //행 인덱스를 증가시켜줌으로 바운더리 값을 설정해 줘야하기 때문이다
                //바운더리는 빙하 1개당 가지고있는 그리드 인덱스이다
                //기획서 1 참고
                //TODO: 궁금한점 0번째 빙하 바운더리 인덱스면 (1 2 3), (11 12 13), (21 22 23) 이되어야하는거아닌가??
                column += 20;
            }


            //빙하 바운더리 인덱스 4번째 방의 원소를 얻어온다.
            //4번째 방은 빙하가 가지고있는 바운더리 인덱스중에 가운데 인덱스이다
            //총 9개의 바운더리 안의 가운데 인덱스라 보면된다 
            int idx = glacier.boundaryIndexes[4];

            //가운데 인덱스이니 빙하 정가운데의 포지션을 얻어오는것
            Vector2 pos = cellDatas[idx].cellPosition;

            //프리팹으로 생성시켜준 타일을 관리하고 빙하의 값들을 얻어오기 위해 저장
            glaciersTiles.Add(glacier);

            //하이어라키창을보니 Grid하위로 글레시아 오브젝트가 있다 백그라운드가 아마 Grid역할을 하는거지않을까 싶다
            obj.transform.SetParent(backGround.transform);

            //빙하의 로컬포지션을 4번째 바운더리 인덱스의 포지션을 설정해주는것
            obj.transform.localPosition = pos;

            //오브젝트의 이름을 설정해주기위한 작업 glaciersTiles의 카운트가 1이되니
            //0부터 이름을 지어주기 위해 이러한 방식 채택으로 예상됨
            obj.name = $"Glacier_{glaciersTiles.Count - 1}";

            //빙하 컴포넌트를 초기화하면서 빙하 타입을 결정해주고 메쉬렌더러의 렌더 순서를 결정해줌
            glacier.Initialize();

            //센터 피봇값 셋팅
            glacier.centerPivot = pos;

            //빙하 인덱스를 0번재부터 시작하기 위한 작업 이름 설정해주는거와 동일
            glacier.glacierIndex = glaciersTiles.Count - 1;

            glacier.glacierDirType = data.directionType;
        }

        public void SetMonsterPathGrid(TileData data)
        {
            //그리드 상 몬스터가 지나갈수 있는 그리드 인덱스를 얻어오는 작업
            int idx = data.startingPoint;

            //셀타입들중 몬스터가 지나갈수있는 그리드를 얻어와서 셀타입을 MOVABLE로 지정
            cellDatas[idx].cellType = CellType.MOVABLE;

            //몬스터가 이동 경로를 그리드 인덱스를 관리하기위한 작업
            monsterPath.Add(idx);
        }

        // DebugMode : 전체 그리드 생성 
        public void SetGrid()
        {
            //width 22
            //height 20
            //총 440칸

            //첫 타일 위치값 (0.52f)값 만큼 감소 -> 5.46 ~ -5.46  (0 ~ 21)
            float offsetY = 5.46f;
            int idx = 0;

            //열 타일 반복
            for (int i = 0; i < width; i++)
            {
                //가로 타일 위치 (0.52f)값 만큼 증가 -4.96 ~ 4.92 (0 ~ 19)
                float offsetX = -4.96f;

                //세로 타일 반복
                for (int j = 0; j < height; j++)
                {
                    //셀 인덱스 및 포지션 및 타입 지정
                    //Debug : 실제 타일 오브젝트로 변환 되어야함
                    CellData data = new()
                    {
                        cellIndex = idx,
                        cellPosition = new Vector2(offsetX, offsetY),
                        cellType = CellType.NONE
                    };
                    //만들어둔 셀 데이터를 저장
                    cellDatas.Add(data);

                    //셀마다의 포지션값을 다르게 지정해줘야하기에 값 지정
                    offsetX += 0.52f;

                    //인덱스 증가
                    idx++;
                }

                //셀마다 포지션값을 다르게 지정해줘야하기에 값 지정
                offsetY -= 0.52f;
            }

            //타일을 생성과 동시에 타입을 빙하 타입인지 몬스터가 지나가는 타입에 따른 타일값을 셋팅해주기위한 함수 
            GetStageData();
        }

        public bool IsPossibleSummon()
        {
            //함수의도 : 소환가능 상태의 빙하를 찾는 조건
            //소환가능 상태의 빙하를 찾으면 캐릭터를 소환해준다던지 그러한 조건을 들고있음

            //소환가능 상태 디폴트 
            bool isPossibleSummon = false;

            //빙하 타일들을 순회
            for (int i = 0; i < glaciersTiles.Count; i++)
            {
                //만약 소환 가능 상태의 빙하가있다면?
                if (!glaciersTiles[i].isImpossibleSummon)
                {
                    //소환가능 상태로 값을 변경해줌
                    isPossibleSummon = true;
                    break;
                }
                else
                {
                    isPossibleSummon = false;
                }
            }
            //소환가능한 조건을 리턴해줌
            return isPossibleSummon;
        }

        public Glacier PossibleSummonIdx()
        {
            //함수의도 : 소환가능 빙하 컴포넌트를 랜덤으로 얻어오기위함
            //무한 반복을 막기위해 바로 위 함수를 외부에서 불러와서 체크를 해줌

            while (true)
            {
                //0부터 현재 빙하타일의 랜덤값을 얻어옴
                int idx = Random.Range(0, glaciersTiles.Count);

                //만약 소환가능 상태의 빙하면 조건 성립
                if (!glaciersTiles[idx].isImpossibleSummon)
                {
                    Glacier glacier = glaciersTiles[idx];
                    return glacier;
                }
                //아니라면 반복
            }
        }

        public void SetDamageAction()
        {
            //호출 위치 : 캐릭터 인덱스가 키링이라면 호출해줌

            //데미지를 입으면 콜백해줄 함수를 재셋팅 기존 함수: NormalDamageAction 변경 함수: PostDamageAction

            damageAction = null;
            damageAction = PostDamageAction;
        }

        public void NormalDamageAction()
        {
            //빙하가 데미지를 입으면 호출되는 콜백함수
            while (true)
            {
                //랜덤으로 안깨진 빙하를 지속적으로 찾아서 데미지를 입히면 반복중단
                int idx = Random.Range(0, glaciersTiles.Count);
                if (glaciersTiles[idx].glacierState != GlacierState.DESTROYED)
                {
                    if (false == glaciersTiles[idx].Shield())
                        glaciersTiles[idx].Damage();

                    break;
                }
            }
        }

        public int GetBorkenGlacierIdx()
        {
            int findIdx = -1;
            bool isGlacierBroken = false;

            for (int i = 0; i < glaciersTiles.Count; i++)
            {
                if (glaciersTiles[i].glacierState == GlacierState.BROKEN)
                {
                    isGlacierBroken = true;
                    break;
                }
            }

            while (isGlacierBroken)
            {
                int idx = Random.Range(0, glaciersTiles.Count);

                if (glaciersTiles[idx].glacierState == GlacierState.BROKEN)
                {
                    findIdx = idx;
                    break;
                }
            }

            return findIdx;
        }

        public void PostDamageAction()
        {
            //int ran = Random.Range(0, 2);

            //CharacterIndex characterIndex = ran == 0 ? CharacterIndex.KIRING : CharacterIndex.DAVI;

            //키링이 덱에있고 소환되어있으면 키링의 빙하 인덱스들을 받아오는 작업
            List<int> data = GameManager.Instance.characterSpawner.GetGlacierCharacterSet(CharacterIndex.KIRING);

            //만약 찾지못했다면
            if (data.Count == 0)
            {
                //기본 데미지 콜백함수 호출
                NormalDamageAction();
            }
            else
            {
                while (true)
                {
                    //만약 찾았다면 랜덤으로 키링이 소환된 빙하의 인덱스를 얻어온다
                    int idx = Random.Range(0, data.Count);

                    //만약 키링이 소환된 빙하의 스테이트를 확인해서 깨져있지 않다면 조건성
                    if (glaciersTiles[data[idx]].glacierState != GlacierState.DESTROYED)
                    {
                        //찾은 빙하 타일에 데미지 함수 호출
                        if (!glaciersTiles[data[idx]].Shield())
                            glaciersTiles[data[idx]].Damage();

                        break;
                    }
                }
            }
        }

        public void TutorialDamage()
        {
            glaciersTiles[18].Damage();
        }

        public void Damage()
        {
            //몬스터가 도착지점에 오면 죽어야하고 빙판을 깨야하기때문에 존재함

            if (isGameOver) return;

            damageAction?.Invoke();

            for (int i = 0; i < glaciersTiles.Count; i++)
            {
                if (glaciersTiles[i].glacierState != GlacierState.DESTROYED)
                {
                    isGameOver = false;
                    break;
                }
                else
                {
                    isGameOver = true;
                }
            }

            if (isGameOver)
            {
                GameManager.Instance.SetGameOverRecordDataClear();
                GameManager.Instance.SetRecordClear();
                GameManager.Instance.GameOver();
            }
        }
        
        public IEnumerator FieldBossDamageSequence()
        {
            for (int i = 0; i < glaciersTiles.Count; i++)
            {
                if (glaciersTiles[i].glacierState != GlacierState.DESTROYED)
                {
                    glaciersTiles[i].FieldBossDestroyedTile();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            GameManager.Instance.GameOver();
        }

        public void FieldBossDamage()
        {
            StartCoroutine(FieldBossDamageSequence());
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}
