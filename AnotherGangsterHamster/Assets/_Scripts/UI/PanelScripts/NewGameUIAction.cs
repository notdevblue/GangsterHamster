using Stages.Management;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PanelScripts
{
    public class NewGameUIAction : UIAction
    {
        [Header("각자의 기능이 있는 UI들")]
        public Button disableButton;
        public Button acceptButton;

        public override void ActivationActions()
        {

        }

        public override void DeActivationActions()
        {

        }

        public override void InitActions()
        {
            panelId = 2;

            disableButton.onClick.AddListener(() =>
            {
                UIManager.Instance.DeActivationPanel(panelId);
            });

            acceptButton.onClick.AddListener(() =>
            {
                StageManager.Instance.Load(StageNames.StoryStage_0_1.ToString());
                // 기존의 저장 데이터 모두 삭제, 게임을 처음부터 재시작 후 "In Game UI"를 활성화
            });
        }

        public override void UpdateActions()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.DeActivationPanel(panelId);
            }
        }
    }
}
