import {buildModule} from "@nomicfoundation/hardhat-ignition/modules"

export default buildModule("MyContractModule", (m)=>{
    const myContract = m.contract("MyNFTContract");

    return {myContract};
})