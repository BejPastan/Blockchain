// SPDX-License-Identifier: MIT
pragma solidity ^0.8.34;

import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/utils/ReentrancyGuard.sol";

contract MyNFTContract is ReentrancyGuard, ERC721Enumerable {
    address public creator;
    uint256 public nextTokenId;

    mapping(uint256 => uint256) public tokenPrices;

    modifier onlyCreator() {
        require(msg.sender == creator, "only creator");
        _;
    }    

    constructor() ERC721("GameItem", "ITM") {
        creator = msg.sender;
    }

    function mintNFT(address recipient, uint256 price) public {
        _safeMint(recipient, nextTokenId);
        tokenPrices[nextTokenId] = price;
        nextTokenId++;
    }

    function addTokenPrice(uint256 tokenId, uint256 price) public onlyCreator {
        tokenPrices[tokenId] = price;
    }

    function getAllTokensWithPrices() public view returns (uint256[] memory tokenIds, uint256[] memory prices)
    {
        uint256 supply = totalSupply();
        tokenIds = new uint256[](supply);
        prices = new uint256[](supply);

        for (uint256 i = 0; i < supply; i++) {
            uint256 tokenId = tokenByIndex(i);
            tokenIds[i] = tokenId;
            prices[i] = tokenPrices[tokenId];
        }

        return (tokenIds, prices);
    }

    function changePrice(uint256 tokenId, uint256 newPrice) public {
        address owner = super.ownerOf(tokenId);
        if(owner == msg.sender)
        {
            tokenPrices[tokenId] = newPrice;
        }
    }

    function removeTokenPrice(uint256 tokenId) public onlyCreator {
        address owner = super.ownerOf(tokenId);
        if(owner == msg.sender)
        {
            delete tokenPrices[tokenId];
        }

    }

    function getTokenPrice(uint256 tokenId) public view returns (uint256) {
        return tokenPrices[tokenId];
    }

    function _update(address to, uint256 tokenId, address auth) internal virtual override returns (address) {
        address previousOwner = super._update(to, tokenId, auth);
        
        if (to == address(0)) {
            delete tokenPrices[tokenId];
        }
        return previousOwner;
    }

    function buyAndTransfer(uint256 tokenId, address to) public payable nonReentrant {
        address owner = ownerOf(tokenId);

        require(msg.value >= tokenPrices[tokenId], "price not enough");

        uint256 royalty = (msg.value * 50) / 10000;
        uint256 sellerAmount = msg.value - royalty;

        _safeTransfer(owner, to, tokenId, "");

        (bool success_royal, ) = payable(creator).call{value: royalty}("");
        (bool success_to, ) = payable(owner).call{value: sellerAmount}("");

        require(success_royal && success_to, "transfer failed");
    }
}