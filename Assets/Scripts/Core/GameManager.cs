using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using gameracers.Architecture;
using System;
using gameracers.Control;
using UnityEngine.SceneManagement;

namespace gameracers.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager gm;
        public GameState state;
        bool hasSpawned = false;
        [SerializeField] GameObject startCutscene;
        [SerializeField][Header("Order: [Victory, Game Over, Death, Pause, Options]")]
        GameObject[] menus;
        GameObject camRot;
        List<BuildingOccupant> occupants = new List<BuildingOccupant>();
        List<GameObject> outsidePeople = new List<GameObject>();
        List<GameObject> quests = new List<GameObject>();
        GameObject player;

        int score = 0;

        public static event Action<GameState> OnGameStateChange;
        
        public class BuildingOccupant
        {
            public string buildingID;
            public HumanController personObj;

            public BuildingOccupant(string buildingID, HumanController personObj)
            {
                this.buildingID = buildingID;
                this.personObj = personObj;
            }
        }

        private void OnEnable()
        {
            EventListener.onCrossThreshold += EntityCrossThreshold;
            EventListener.onQuest += QuestComplete;
            EventListener.onPause += Pausing;
            EventListener.onDemonButton += DemonButton;
        }

        private void OnDisable()
        {
            EventListener.onCrossThreshold -= EntityCrossThreshold;
            EventListener.onQuest -= QuestComplete;
            EventListener.onPause -= Pausing;
            EventListener.onDemonButton -= DemonButton;
        }

        private void EntityCrossThreshold(GameObject entity, GameObject building, bool isEnter)
        {
            if (GameObject.ReferenceEquals(entity, player))
            {
                return;
            }

            if (isEnter == true)
            {
                if (entity.GetComponent<HumanController>() != null)
                {
                    BuildingOccupant person = new BuildingOccupant(building.GetComponent<Building>().GetBuildingID(), entity.GetComponent<HumanController>());
                    occupants.Add(person);
                    outsidePeople.Remove(entity);
                }
            }
            if (isEnter == false)
            {
                for (int i = 0; i < occupants.Count; i++)
                {
                    if (occupants[i].buildingID == building.GetComponent<Building>().GetBuildingID())
                    {
                        if (GameObject.ReferenceEquals(occupants[i].personObj, entity))
                        {
                            occupants.RemoveAt(i);
                            outsidePeople.Add(entity);
                            return;
                        }
                    }
                }
            }
        }

        private void QuestComplete(string obj, bool isComplete)
        {

        }

        private void Pausing(bool setPause)
        {
            if (setPause == true)
                UpdateGameState(GameState.Pause);
            else
                UpdateGameState(GameState.Play);
        }

        private void DemonButton(string id)
        {
            foreach (BuildingOccupant bo in occupants)
            {
                if (bo.buildingID == id)
                    bo.personObj.SetDemonAlert();
            }
        }

        void Awake()
        {
            gm = this;
            player = GameObject.FindGameObjectWithTag("Player");
            outsidePeople = GameObject.FindGameObjectsWithTag("Human").ToList<GameObject>();
            camRot = GameObject.Find("Camera Rotation");
        }

        private void Start()
        {
            UpdateGameState(state);
            //SaveGame();
        }

        public void UpdateGameState(GameState newState)
        {
            state = newState;
            switch (newState)
            {
                case GameState.Spawning:
                    StartCinemaCam();
                    break;
                case GameState.Play:
                    PlayGame();
                    break;
                case GameState.Pause:
                    PauseGame();
                    break;
                //case GameState.Save:
                //    SaveGame();
                //    break;
                //case GameState.Load:
                //    LoadGame();
                //    break;
                case GameState.PhotoMode:
                    PhotoMode();
                    break;
                case GameState.Lose:
                    GameOver(false);
                    break;
                case GameState.Victory:
                    GameOver(true);
                    break;
            }
            OnGameStateChange?.Invoke(state);
            Debug.Log(state.ToString());
        }

        private void StartCinemaCam()
        {
            if (startCutscene != null)
            {
                player.GetComponent<PlayerBrain>().enabled = false;
                startCutscene.SetActive(true);
                Cursor.visible = false;
            }
        }

        private void PlayGame()
        {
            player.GetComponent<PlayerBrain>().enabled = true;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            menus[3].SetActive(false);
            menus[4].SetActive(false);
            //camRot.SetActive(true);
        }

        private void PauseGame()
        {
            menus[3].SetActive(true);
            menus[4].SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            //camRot.SetActive(false);
            Time.timeScale = 0f;
        }

        public void SaveGame()
        {
            Debug.Log("TODO: Save Game");
        }

        public void LoadGame()
        {
            Debug.Log("TODO: Load Game");
        }

        public void PhotoMode()
        {
            Debug.Log("TODO: Photo Mode");
        }

        public void GameOver(bool isSuccessful)
        {
            if (isSuccessful == true)
            {
                menus[0].SetActive(true);
            }
            else
            {
                menus[1].SetActive(true);
            }
        }

        public void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public enum GameState
    {
        Spawning, //Player Spawns and controls disabled during spawn anim and intro duration as camera flies around
        Play,
        Pause,
        //Save,
        //Load,
        PhotoMode,
        Lose,
        Victory
    }
}