mergeInto(LibraryManager.library, {
SolanaLogin__deps: ['SolanaTokens'],
Vlawmz__deps: ['SendSolTokens'],
SolanaTokens__deps: ['Vlawmz'],

  // Initial login using Phantom

  GetSolanaWebJS: function() {
    console.log("Adding SolanaWeb3 JS to page");
    var script = document.createElement("script");
    script.src = "https://unpkg.com/@solana/web3.js@1.41.0/lib/index.iife.js";
   
    document.head.appendChild(script);
  },

  SolanaLogin: async function(service, cluster, cb) {
    this.endpoint = UTF8ToString(cluster);
    this.connection = new solanaWeb3.Connection(endpoint);
    this.provider = UTF8ToString(service);
    this.wallet = null;
    this.myKey = null;
    this.myTokens = null;

    console.log("Chosen provider is - " + this.provider);
    if (this.provider == "phantom") {
      const isPhantomInstalled = window.solana && window.solana.isPhantom
      if (window.solana.isConnected === false && isPhantomInstalled) {
          const resp = await window.solana.connect();
        }
        else if(window.solana.isConnected === true && isPhantomInstalled){
        
          const resp = await window.solana.disconnect();

          this.myKey = null;
          return;
        }
      else { window.open("https://phantom.app/", "_blank"); Module.dynCall_vi(cb, null); return;}
      const pubKey = await window.solana.publicKey;
      if (pubKey == null) {
        Module.dynCall_vi(cb, null);
        return;
      }
      this.myKey = pubKey.toString();
    }
    if (this.provider == "solflare") {
      const isSolflareInstalled = window.solflare && window.solflare.isSolflare;
      if (window.solflare.isConnected === false && isSolflareInstalled) {
          const resp = await window.solflare.connect();
        }
        else if(window.solana.isConnected === true && isSolflareInstalled){
        
          const resp = await window.solflare.disconnect();
          
          this.myKey = null;

          return;
        
        }
      else { window.open('https://solflare.com', '_blank'); Module.dynCall_vi(cb, null); return;}
      console.log(window.solflare.publicKey);
      const pubKey = await window.solflare.publicKey;
      if (pubKey == null) {
        Module.dynCall_vi(cb, null);
        return;
      }
      this.myKey = pubKey.toString();
    }
    
    _SolanaTokens();

    // This part is for the callback for the connection passing back the wallet address
    var str2 = this.myKey;
        var len2 = lengthBytesUTF8(str2) + 1;
        var strPtr2 = _malloc(len2);
        stringToUTF8(str2, strPtr2, len2);
        Module.dynCall_vi(cb, strPtr2);
        return;
  },

  // Grabs meta keys and receives a list of NFT URIs, Credits to Vlawmz for the massive help https://www.twitter.com/flawmz

  Vlawmz: async function() {
    let meta_keys = [];
    meta_program = new solanaWeb3.PublicKey('metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s');

    for (let i = 0, l = myTokens.length; i < l; i++) {
        let ta = myTokens[i].account.data.parsed.info;
        let mint_key = new solanaWeb3.PublicKey(ta.mint);

        meta_keys.push((await solanaWeb3.PublicKey.findProgramAddress(
            [
                new Uint8Array([109,101,116,97,100,97,116,97]),
                meta_program.toBuffer(),
                mint_key.toBuffer()
            ],
            meta_program
        ))[0].toBase58());
    }
    meta_keys = meta_keys.map((e) => new solanaWeb3.PublicKey(e));
    let metadata_accounts = await connection.getMultipleAccountsInfo(meta_keys.slice(0,100));
    let http = "104116116112115584747";
    let urls = metadata_accounts
  .filter(e => e !== null)
  .map((e) => e.data)
  .map((e) => {
    let arr = Array.from(e);
    let ret = [];
    let push = false;
    for (let i = 0, l = arr.length; i < l; i++) {
      let test = arr.slice(i, i + 8);
      if (test.join('') == http) {
        push = true;
      }

      if (push) {
        if (arr[i] === 0) {
          push = false;
        } else {
          ret.push(arr[i]);
        }
      }
    }
    return ret;
  })
  .map((e) => e.map((e2) => String.fromCharCode(e2)).join(''));
    this.tokenList = urls;
    return _SendSolTokens();
  },

  SolanaTokens: async function() {
    console.log("Your Public Key is ", myKey);
    var url = endpoint;

    var xhr = new XMLHttpRequest();
    xhr.open("POST", url);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = async function () {
      if (xhr.readyState === 4) {
        const tokenResponse = JSON.parse(xhr.responseText);  
        myTokens = await tokenResponse.result.value;

        for (var i = 0; i < myTokens.length; i++) {
          if (myTokens[i].account.data.parsed.info.tokenAmount.amount > 0) {
          Module.ccall("SendMessageString", null, ["string", "string", "string"], ["CryptoReceiver", "ReceiveSPLTokens", myTokens[i].account.data.parsed.info.mint + "|" + myTokens[i].account.data.parsed.info.tokenAmount.uiAmount + "|" + myTokens[i].account.data.parsed.info.tokenAmount.decimals]);
          }
          if (
            myTokens[i].account.data.parsed.info.tokenAmount.decimals == 0 &&
            myTokens[i].account.data.parsed.info.tokenAmount.amount != 1
          ) {
            myTokens.splice(i, 1);
          }
        }

        return _Vlawmz();
      }
    };
    var data = `
              {
                "jsonrpc": "2.0",
                "id": 1,
                "method": "getTokenAccountsByOwner",
                "params": [
                  "${this.myKey}",
                  {
                    "programId": "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA"
                  },
                  {
                    "encoding": "jsonParsed"
                  }
                ]
              }
            `;

    xhr.send(data);
  },

  // Finally sends a request to each URI and parses the JSON response then sends each NFT to the CryptoReceiver object

  SendSolTokens: async function() {
    var tokenLength = tokenList.length;
    for (var i = 0; i < tokenLength; i++) {          
      fetch(tokenList[i])
      .then(res => res.json())
      .catch(function () {
            console.log("URI was unreachable due to CORS issue");
            return;
      })
      .then(data => {
        if (data == null) { return; }
        var attributes = "";
        if (data.attributes.length >= 0) {
          for (var i = 0; i < data.attributes.length; i++) {
            if (i != 0) {
              attributes += "|"
            }
            attributes += data.attributes[i].trait_type + "~" + data.attributes[i].value;
          }
        }
        Module.ccall("SendMessageString", null, ["string", "string", "string"], ["CryptoReceiver", "ReceiveNFT", data.name + "¬" + data.image + "¬" + data.description + "¬" + attributes]);
      });          
    }
    for (var t = 0; t < myTokens.Length; t++) {
      Module.ccall("SendMessageString", null, ["string", "string", "string"], ["CryptoReceiver", "ReceiveNFTMints", myTokens[t].account.data.parsed.info.mint]);
    }
  },

  // Testing Solana transaction

  SendSolTransaction: async function(destination, amount, callback) {
    let goingTo = UTF8ToString(destination)
    let transaction = new solanaWeb3.Transaction().add(
    solanaWeb3.SystemProgram.transfer({
      fromPubkey: window.solana.publicKey,
      toPubkey: new solanaWeb3.PublicKey(goingTo),
      lamports: (solanaWeb3.LAMPORTS_PER_SOL * amount),
    })
    );
    let { blockhash } = await connection.getRecentBlockhash();
    transaction.recentBlockhash = blockhash;
    transaction.feePayer = window.solana.publicKey;
    let signed = await window.solana.signTransaction(transaction);
    let txid = await connection.sendRawTransaction(signed.serialize());
    const confirm = await connection.confirmTransaction(txid);
    console.log("Your transaction ID is " + txid);

    const status = await connection.getSignatureStatuses([txid]);
    if (status.value[0].confirmationStatus === "finalized") {
      confirmed = true;
    }
    else {
      confirmed = false;
    }
    console.log("Transaction Finalized = " + confirmed);
    console.log("calling back");
    Module['dynCall_vi'](callback, confirmed);

  }
})