/*!
 * (C) Ionic http://ionicframework.com - MIT License
 */
import{a as o,w as s}from"./p-IGIE5vDm.js";import{f as t,s as r}from"./p-B8xlpH8p.js";import{c as a}from"./p-CGmVTdWh.js";const m=()=>{const m=window;m.addEventListener("statusTap",(()=>{o((()=>{const o=document.elementFromPoint(m.innerWidth/2,m.innerHeight/2);if(!o)return;const n=t(o);n&&new Promise((o=>a(n,o))).then((()=>{s((async()=>{n.style.setProperty("--overflow","hidden"),await r(n,300),n.style.removeProperty("--overflow")}))}))}))}))};export{m as startStatusTap}