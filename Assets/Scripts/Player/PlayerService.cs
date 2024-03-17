using System.Collections.Generic;
using UnityEngine;
using ServiceLocator.Player.Projectile;
using ServiceLocator.Utilities;
using ServiceLocator.UI;
using ServiceLocator.Map;
using ServiceLocator.Sound;

namespace ServiceLocator.Player {
    public class PlayerService {
        public PlayerScriptableObject playerScriptableObject;

        private ProjectilePool projectilePool;

        private List<MonkeyController> activeMonkeys;
        private MonkeyView selectedMonkeyView;
        private int health;
        private int money;
        public int Money => money;

        private void Start() {
            projectilePool = new ProjectilePool(playerScriptableObject.ProjectilePrefab, playerScriptableObject.ProjectileScriptableObjects);
            InitializeVariables();
        }

        public PlayerService(PlayerScriptableObject pso) {
            playerScriptableObject = pso;
            Start();
        }

        private void InitializeVariables() {
            health = playerScriptableObject.Health;
            money = playerScriptableObject.Money;
            GameService.Instance.uiService.UpdateHealthUI(health);
            GameService.Instance.uiService.UpdateMoneyUI(money);
            activeMonkeys = new List<MonkeyController>();
        }

        public void Update() {
            if (activeMonkeys.Count > 0) {
                foreach (MonkeyController monkey in activeMonkeys) {
                    monkey.UpdateMonkey();
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                UpdateSelectedMonkeyDisplay();
            }
        }

        private void UpdateSelectedMonkeyDisplay() {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

            foreach (RaycastHit2D hit in hits) {
                if (IsMonkeyCollider(hit.collider)) {
                    selectedMonkeyView?.MakeRangeVisible(false);
                    selectedMonkeyView = hit.collider.GetComponent<MonkeyView>();
                    selectedMonkeyView.MakeRangeVisible(true);
                    return;
                }
            }

            selectedMonkeyView?.MakeRangeVisible(false);
        }

        private bool IsMonkeyCollider(Collider2D collider) => collider != null && !collider.isTrigger && collider.GetComponent<MonkeyView>() != null;

        public void ValidateSpawnPosition(int monkeyCost, Vector3 dropPosition) {
            if (monkeyCost > Money)
                return;

            MapService.Instance.ValidateSpawnPosition(dropPosition);
        }

        public void TrySpawningMonkey(MonkeyType monkeyType, int monkeyCost, Vector3 dropPosition) {
            if (monkeyCost > money)
                return;

            if (MapService.Instance.TryGetMonkeySpawnPosition(dropPosition, out Vector3 spawnPosition)) {
                SpawnMonkey(monkeyType, spawnPosition);
                GameService.Instance.soundService.PlaySoundEffects(SoundType.SpawnMonkey);
            }
        }

        public void SpawnMonkey(MonkeyType monkeyType, Vector3 spawnPosition) {
            MonkeyScriptableObject monkeySO = playerScriptableObject.MonkeyScriptableObjects.Find(so => so.Type == monkeyType);
            MonkeyController monkey = new MonkeyController(monkeySO, projectilePool);
            monkey.SetPosition(spawnPosition);
            activeMonkeys.Add(monkey);

            money -= monkeySO.Cost;
            GameService.Instance.uiService.UpdateMoneyUI(money);
        }

        public void ReturnProjectileToPool(ProjectileController projectileToReturn) => projectilePool.ReturnItem(projectileToReturn);

        public void TakeDamage(int damageToTake) {
            health = health - damageToTake <= 0 ? 0 : health - damageToTake;
            GameService.Instance.uiService.UpdateHealthUI(health);
            if (health <= 0) {
                PlayerDeath();
            }
        }

        public void GetReward(int reward) {
            money += reward;
            GameService.Instance.uiService.UpdateMoneyUI(money);
        }

        private void PlayerDeath() => GameService.Instance.uiService.UpdateGameEndUI(false);
    }
}