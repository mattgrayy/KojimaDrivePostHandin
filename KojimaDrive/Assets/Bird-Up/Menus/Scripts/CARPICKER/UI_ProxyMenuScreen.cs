//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Forward inputs on to other menuscreens
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_ProxyMenuScreen : BaseMenuScreen {
		public BaseMenuScreen[] m_Proxies;

		bool CheckProxyValid(BaseMenuScreen menu, int nPlayerID) {
			return (menu != null && (menu.m_nTargetPlayerID == nPlayerID || menu.m_nTargetPlayerID == 0));
		}

		public override void Up(int nPlayerID) {
			for (int i = 0; i < m_Proxies.Length; i++) {
				if(CheckProxyValid(m_Proxies[i], nPlayerID)) {
					m_Proxies[i].Up(nPlayerID);
				}
			}
		}

		public override void Down(int nPlayerID) {
			for (int i = 0; i < m_Proxies.Length; i++) {
				    if(CheckProxyValid(m_Proxies[i], nPlayerID)) {
					m_Proxies[i].Down(nPlayerID);
				}
			}
		}

		public override void Left(int nPlayerID) {
			for (int i = 0; i < m_Proxies.Length; i++) {
				    if(CheckProxyValid(m_Proxies[i], nPlayerID)) {
					m_Proxies[i].Left(nPlayerID);
				}
			}
		}

		public override void Right(int nPlayerID) {
			for (int i = 0; i < m_Proxies.Length; i++) {
				    if(CheckProxyValid(m_Proxies[i], nPlayerID)) {
					m_Proxies[i].Right(nPlayerID);
				}
			}
		}

		public override void Select(int nPlayerID) {
			for (int i = 0; i < m_Proxies.Length; i++) {
				    if(CheckProxyValid(m_Proxies[i], nPlayerID)) {
					m_Proxies[i].Select(nPlayerID);
				}
			}
		}

		public override void Cancel(int nPlayerID) {
			for (int i = 0; i < m_Proxies.Length; i++) {
				    if(CheckProxyValid(m_Proxies[i], nPlayerID)) {
					m_Proxies[i].Cancel(nPlayerID);
				}
			}
		}
	}
}