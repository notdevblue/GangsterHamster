using Matters.Gravity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Weapons.Actions;

namespace Weapons.Animation
{
    public class LumoAnimator : MonoBehaviour
    {
        private Transform _mainCameraTransform;
        private Transform MainCameraTransform
        {
            get
            {
                if (_mainCameraTransform == null)
                {
                    _mainCameraTransform = Camera.main.transform;
                }

                return _mainCameraTransform;
            }
        }

        enum LumoAnimeStatus // 현재 애니메이션 스테이터스
        {
            Idle,
            Move,
            Reset,
            Using,
            Sorting
        }

        public float usingAnimeDelay = 0.5f;                    // 애니메이션을 시작하는데 생기는 딜레이
        public float sortPartsTime = 2f;                    // 애니메이션을 멈추는데 생기는 딜레이
        public float resetMultiply = 2f;
        [SerializeField] private float rotSpeed = 1f;                                 // 돌아가는 회전 속도

        public UnityEvent StartAnimationCallback;
        public UnityEvent StopAnimationCallback;

        public List<GameObject> clockwiseRotObjs = new List<GameObject>();
        public List<GameObject> antiClockwiseRotObjs = new List<GameObject>();

        public Vector3 clockwiseRotDir;
        public Vector3 antiClockwiseRotDir;

        public Vector3 lumoIdleLookVec;

        private List<Vector3> _partRotDirList = new List<Vector3>();    // 파츠마다의 애니메이션 회전 값
        private List<Transform> _partTrmList = new List<Transform>();
        private LumoAnimeStatus _curStatus = LumoAnimeStatus.Idle;    // 지금 애니메이션 스테이터스

        private Quaternion quaternion;
        private Vector3 start;                                              // 애니메이션 실행시 시작 위치
        private Vector3 end;                                                // 애니메이션 실행시 끝나는 위치
        private float moveSpeed;                                            // 애니메이션 실행 스피드
        
        private float _curTime;                                     // 시간 계산을 위한 변수
        private float _curSortingTime;                              // 정렬 계산을 위한 변수
        private float _softMoving;                                  // 부드럽게 움직이기 위하여 사용하는 변수
        private int childCount;                                     // 자식의 수 - 그냥 많이 쓰여서 따로 변수로 뺌
        private bool isEnd = false; 
        private bool isReset = true;

        private Lumo _lumo;

        private void Awake()
        {
            childCount = 9;

            // 시작시 랜덤으로 파츠마다의 랜덤 회전 값 지정
            for (int i = 0; i < childCount; i++)
            {
                _partRotDirList.Add(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                _partTrmList.Add(transform.GetChild(i));
            }

            _lumo = GetComponent<Lumo>();
            quaternion = Quaternion.Euler(90, 0, 0);
        }

        public void FireAnime(Vector3 start, Vector3 end, float moveSpeed, Quaternion quaternion)
        {
            transform.rotation = MainCameraTransform.rotation;

            // Move -> Idle 로 상태 변환
            InitAnime(start, end, moveSpeed, LumoAnimeStatus.Move, quaternion);
            isReset = false;
            isEnd = false;
        }
        
        public void ResetAnime(Vector3 start, Vector3 end, float moveSpeed)
        {
            // Reset -> Idle 로 상태 변환
            InitAnime(start, end, moveSpeed, LumoAnimeStatus.Reset, Quaternion.Euler(new Vector3(90, 0, 0)));
            isReset = true;
            isEnd = false;
            _curSortingTime = 0;
        }

        public void UsingAnime(Vector3 normalVec, float moveSpeed)
        {
            // Using -> 계속 유지함
            InitAnime(transform.position, transform.position + normalVec, moveSpeed, LumoAnimeStatus.Using, Quaternion.identity);
            _softMoving = 0f;
            isEnd = false;
        }

        public void PositionReset(Vector3 pos)
        {
            transform.position = pos;
            _curStatus = LumoAnimeStatus.Idle;
            isReset = true;
        }

        public void RotationReset()
        {
            for (int i = 0; i < clockwiseRotObjs.Count; i++)
            {
                clockwiseRotObjs[i].transform.rotation = Quaternion.Euler(90, 0, 0);
            }

            for (int i = 0; i < antiClockwiseRotObjs.Count; i++)
            {
                antiClockwiseRotObjs[i].transform.rotation = Quaternion.Euler(90, 0, 0);
            }

            _curStatus = LumoAnimeStatus.Idle;
            isReset = true;
        }

        public bool isStopedMoving()
        {
            return _curStatus == LumoAnimeStatus.Idle || isEnd;
        }

        private void Update()
        {
            Sorting();

            switch (_curStatus)
            {
                case LumoAnimeStatus.Idle:
                    if(isReset)
                    {
                        transform.position = _lumo.GetHandPos;

                        transform.rotation = Quaternion.Lerp(transform.rotation, MainCameraTransform.rotation * Quaternion.Euler(lumoIdleLookVec), 0.5f);
                    }
                    break;
                case LumoAnimeStatus.Move:
                    _curTime += Time.deltaTime * moveSpeed;

                    if (_curTime >= Vector3.Distance(start, end))
                    {
                        // 발사 위치로 이동 완료
                        transform.position = end;
                        _curStatus = LumoAnimeStatus.Idle;
                        _curTime = 0;

                        for (int i = 0; i < childCount; i++)
                        {
                            _partTrmList[i].localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                        }
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(start, end, _curTime / Vector3.Distance(start, end)); 
                    }
                    break;
                case LumoAnimeStatus.Reset:
                    _curTime += Time.deltaTime * moveSpeed * resetMultiply;

                    if (_curTime >= Vector3.Distance(start, _lumo.GetHandPos))
                    {
                        // 발사 위치로 이동 완료
                        transform.position = _lumo.GetHandPos;
                        _curStatus = LumoAnimeStatus.Idle;
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(start, _lumo.GetHandPos, _curTime / Vector3.Distance(start, _lumo.GetHandPos));
                    }
                    break;

                case LumoAnimeStatus.Using:
                    _curTime += Time.deltaTime / usingAnimeDelay * _softMoving;
                    _softMoving += Time.deltaTime;

                    if (_curTime >= Vector3.Distance(start, end))
                    {
                        // 조금 튀어나옴
                        RotationParts(rotSpeed * Time.deltaTime);
                        isEnd = true;
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(start, end, _curTime / Vector3.Distance(start, end));
                    }
                    break;
            }
        }

        // 들어온 스피드에 따라서 파츠들을 돌리는 함수
        private void RotationParts(float rotSpeed)
        {
            for(int i = 0; i < clockwiseRotObjs.Count; i++)
            {
                clockwiseRotObjs[i].transform.rotation *= Quaternion.Euler(clockwiseRotDir * rotSpeed / (i + 4));
            }

            for (int i = 0; i < antiClockwiseRotObjs.Count; i++)
            {
                antiClockwiseRotObjs[i].transform.rotation *= Quaternion.Euler(antiClockwiseRotDir * rotSpeed / (i + 4));
            }
        }

        private void Sorting()
        {
            if (!isReset || _curSortingTime > sortPartsTime) return;

            _curSortingTime += Time.deltaTime;

            if (_curSortingTime > sortPartsTime)
            {
                for (int i = 0; i < childCount; i++)
                {
                    _partTrmList[i].localRotation = quaternion;
                }
            }
            else
            {
                for (int i = 0; i < childCount; i++)
                {
                    _partTrmList[i].localRotation = Quaternion.Lerp(_partTrmList[i].localRotation, quaternion, _curSortingTime / sortPartsTime);
                }
            }
        }
        private void InitAnime(Vector3 start, Vector3 end, float moveSpeed, LumoAnimeStatus status, Quaternion quaternion)
        {
            _curTime = 0;

            this.start = start;
            this.end = end;
            this.moveSpeed = moveSpeed;
            this.quaternion = quaternion;
            _curStatus = status;
        }
    }
}