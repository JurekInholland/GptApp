<template>
    <label>
        <input type='checkbox'>
        <span class='menu'> <span class='hamburger'></span> </span>
    </label>
</template>

<style scoped>
label .menu {
    position: absolute;
    /* right: -200px; */
    right: 6rem;
    top: -120px;
    z-index: 100;
    background: #FFF;
    border-radius: 50% 50% 50% 50%;
    -webkit-transition: .5s ease-in-out;
    transition: .5s ease-in-out;
    cursor: pointer;
    background-color: #FFF;
    margin-top: 1rem;


}

label .hamburger {
    position: absolute;
    top: 134px;
    left: 51px;
    width: 30px;
    height: 3px;
    background: var(--color-hamburger);
    display: block;
    -webkit-transform-origin: center;
    transform-origin: center;
    -webkit-transition: .5s ease-in-out;
    transition: .5s ease-in-out;
}

label .hamburger:after,
label .hamburger:before {
    -webkit-transition: .5s ease-in-out;
    transition: .5s ease-in-out;
    content: "";
    position: absolute;
    display: block;
    width: 100%;
    height: 100%;
    background: var(--color-hamburger);

}

label .hamburger:before {
    top: -10px;
}

label .hamburger:after {
    bottom: -10px;
}

label input {
    /* display: none; */
    opacity: 0;
    width: 1.8rem;
    height: 1.8rem;
    cursor: pointer;
    margin-top: 1rem;
}

/* label input:checked+.menu {
    box-shadow: 0 0 0 100vw #FFF, 0 0 0 100vh #FFF;
    border-radius: 0;
} */

label input:checked+.menu .hamburger {
    -webkit-transform: rotate(45deg);
    transform: rotate(45deg);
}

label input:checked+.menu .hamburger:after {
    -webkit-transform: rotate(90deg);
    transform: rotate(90deg);
    bottom: 0;
}

label input:checked+.menu .hamburger:before {
    -webkit-transform: rotate(90deg);
    transform: rotate(90deg);
    top: 0;
}

label input:checked+.menu+ul {
    opacity: 1;
}

/* 
label ul {
    z-index: 200;
    position: absolute;
    top: 50%;
    left: 50%;
    -webkit-transform: translate(-50%, -50%);
    transform: translate(-50%, -50%);
    opacity: 0;
    -webkit-transition: .25s 0s ease-in-out;
    transition: .25s 0s ease-in-out;
} */

/* label a {
    margin-bottom: 1em;
    display: block;
    color: #000;
    text-decoration: none;
} */
label .hamburger::before,
label .hamburger,
label .hamburger::after {
    transition: background-color 0.5s ease, transform 0.5s ease, top .5s ease, bottom .5s ease;
}

label:hover .hamburger::before,
label:hover .hamburger,
label:hover .hamburger::after {
    background-color: #667f5d;

}
</style>