# Blockchain Demo — Project Instructions

Dokument wyjaśnia jak 

1. postawić śrowodiwsko do lokalnego developemtnu hardhat, oraz nethereum
2. Włączyć lolakną sieć blockchain, skompilować kontrakt, i publikowaćgo w sieci
3. Pokaże jak łączyć się z siecią, na przykłądzie klas (`Configuration.cs`, `APIWrapper.cs`).
4. Jak tworzyć kontrakt na podstawie`MyNFTContract.sol`.

---

## 0. Instalacja

### 0.1 Hardhat (Node.js)

Hardhat to środowisko do towrzenia kontraktów, oraz testowania ich lokalnie

wwymagane jest posiadanie  **Node.js** (v22.13.0 or later) from https://nodejs.org/

  ```
   mkdir blockchain-demo
   cd blockchain-demo
   npm init -y
   npm install --save-dev hardhat
   npx hardhat init
  ```

   Konfiguracja `hardhat.config.ts`:

   ```ts
   import { defineConfig } from "hardhat/config";

   export default defineConfig({
     solidity: {
       version: "0.8.34",
     },
     networks: {
       localhost: {
         url: "http://localhost:8545",
       },
     },
   });
   ```

### 0.2 Nethereum

Nethereum to biblioteka do łączenia się z sieciami blockchain z posiomu Unity

1. **Window → Package Manager**.
2. **Add a package from git URL...**

   ```
   https://github.com/Nethereum/Nethereum.Unity.git
   ```

### 1.1 lokalne środowisko testowe

W pierwszym terminalu

```bash
npx hardhat node
```

włączy to lokalną sieć blockchain, z 20 testowymi kontami, zawierającymi po 1000ETH

### 1.2 Kompilowanie kontraktu

```bash
npx hardhat compile
```

Hardhat kompiluje wsyzstkie kontrakty `.sol` z folderu `contracts/`

### 1.3 Publikacja kotraktu

tworzenie skryptu do publikacji kontraktu `scripts/deploy.ts`:

```ts
import { HardhatRuntimeEnvironment } from "hardhat/types";
import { DeployFunction } from "hardhat-deploy/types";

const deploy: DeployFunction = async (hre: HardhatRuntimeEnvironment) => {
  const { deployments, getNamedAccounts } = hre;
  const { deploy } = deployments;
  const { deployer } = await getNamedAccounts();

  await deploy("MyNFTContract", {
    from: deployer,
  });
};

export default deploy;
```

W drugiej konsoli wywołaj komendę:

```bash
npx hardhat run scripts/deploy.ts --network localhost
```

Jako odpowiedź dostaniesz adres kontraktu, będzie on potem uzywany do wywoływania funkcji, które są w kontrakcie zdefiniowane


### 2.1 Scripts/Configuration.cs

Projekt Unity posiada ScriptableObject `Settings/MarketConfig`, zawiera on konfigurację łączenia z siecią blockchain, oraz kontrakt. 
Na ten moment zaimplementowana jest tylko opcja lokalna.

Jako parametr `abi` podajemy plik json, wygenerowany w trakice kompilacji kontraktu `artifacts/contracts/{nazwa kontraktu}`. Z pliku usuwamy wszystko poza tablicą `abi`

wersja przed edycją
```json
{
  "_format": "hh3-artifact-1",
  "contractName": "MyNFTContract",
  "sourceName": "contracts/MyNFTContract.sol",
  "abi": [
    {
      "inputs": [],
      "stateMutability": "nonpayable",
      ... reszta tablicy
  ],
  "bytecode": ... reszta pliku
}
```
wersja po edycji
```json
[
    {
      "inputs": [],
      "stateMutability": "nonpayable",
      ... reszta tablicy
  ]
```

### 2.2 `Scripts/APIWrapper.cs`
singleton zawierający funkcję wykorzystywane w demo. Korzysta z biblioteki Nethereum do łączenia z siecią blockchain. 


- `config` Scriptable obejct, zawierający dane konfiguracji(kontrakty)

W demo zostały wykorzystane dwa sposoby na wywoływanie funkcji

```
function.CallAsync<T>(params)
```
służy do wywoływania funkcji odczytu, nie ma możliwości zmiany stanu blochain'u

```
  var transactionHash = await function.SendTransactionAsync(
  mintingUser.PublicKey,//klucz publiczny użytkownika który wywołuje funkcję
  new Nethereum.Hex.HexTypes.HexBigInteger(300000),//maksymalny dopuszczalny koszt wywołanai funkcji
  null,//Value, transakcje były stworzone do płacenia, więc jest to domyślnie płacona cena
  null,
  mintingUser.PublicKey,//parametr 1
  price//parametr 2
  ).ConfigureAwait(false);
```


### 2.3 Disclaymer
Pliki interfejsu, bindowanie danych itp, nie są tutaj istotne, zostały stworzone tylko aby umożliwić testowanie funkcjonalności, i mogą posiadać błędy

### 3
Przykładowy kontrakt, stworzony w celu testowania dostepny jest w folderze `.Dependencies/contracts/MyNFTContract.sol`