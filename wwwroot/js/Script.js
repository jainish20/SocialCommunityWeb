document.addEventListener('DOMContentLoaded', function () {
    const btn = document.getElementById("chirag");

    btn.addEventListener("click", function () {
        const newDiv = '<div id="panchal" class="_2uYY-KeuYHKiwl-9aF0UiL _1HSQGYlfPWzs40LP4_oFi5 _2XkHtsPtFuTExJyk9JQUAp" role="menu" tabindex="-1" style="position: fixed; left: 1264.6px; top: 45.2px;"> <div id="chibg" style="background: white;"> <div class="_3SDj_IT6ZaqCbKfC4eTjb2" >              <a href="/home/userdetail"><button class="_3fbofimxVp_hpVM6I1TGMS GCltVwsXPu5lE-gs4Nucu"><span class="f8nXLisWxOYzMMl1uIAP3"><span class="_225mt8Dkk7IyfYILsMLk9t"><i class="icon icon-user"></i></span><span class="yloKeyD8bfd8UJ_Gi9rsR">Profile</span></span></button></a> </div> <a href="/register/logout"><button class="_3fbofimxVp_hpVM6I1TGMS GCltVwsXPu5lE-gs4Nucu"><span class="f8nXLisWxOYzMMl1uIAP3"><span class="_225mt8Dkk7IyfYILsMLk9t"><i class="icon icon-logout"></i></span><span class="yloKeyD8bfd8UJ_Gi9rsR">Log Out</span></span></button></a> <div class="_2XCnMY85ivEZUL6cAgK0nV"><span class="_ttuvLVH4k74IkCGNFJSt">© 2023 Clique, Inc. All rights reserved</span></div> </div> </div>';
        
        // set the text content of the new div
        
        const d = document.getElementById("panchal");
        if (d) {
            console.log("removing element");
            d.remove();
        } else {
            console.log("adding the element");
            document.getElementById("inserthehe").insertAdjacentHTML('beforeend', newDiv);
            console.log("added");
            
        }
    });
});


