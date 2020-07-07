/* eslint-disable @typescript-eslint/camelcase */
import * as React from "react";
import Particles, { MoveDirection, InteractivityDetect, OutMode } from "react-tsparticles";
import "./particles.css";

export class ParticlesBackgound extends React.Component {
    render() {
        return (
            <Particles
                params={{
                    fpsLimit: 60,
                    particles: {
                        number: {
                            value: 100,
                            density: {
                                enable: true,
                                value_area: 800,
                            },
                        },
                        color: {
                            value: "#AAADB0",
                        },
                        shape: {
                            type: "circle",
                            stroke: {
                                width: 0,
                                color: "#F1F4F6",
                            },
                            polygon: {
                                nb_sides: 5,
                            },
                            image: {
                                src:
                                    "https://cdn.matteobruni.it/images/particles/github.svg",
                                width: 100,
                                height: 100,
                            },
                        },
                        opacity: {
                            value: 0.9,
                            random: true,
                            anim: {
                                enable: true,
                                speed: 3,
                                opacity_min: 0.1,
                                sync: false,
                            },
                        },
                        size: {
                            value: 5,
                            random: true,
                            anim: {
                                enable: true,
                                speed: 20,
                                size_min: 0.1,
                                sync: false,
                            },
                        },
                        line_linked: {
                            enable: true,
                            distance: 150,
                            color: "#AAADB0",
                            opacity: 0.4,
                            width: 1,
                        },
                        move: {
                            enable: true,
                            speed: 1.5,
                            direction: "none" as MoveDirection,
                            random: true,
                            straight: false,
                            out_mode: "destroy" as OutMode,
                            attract: {
                                enable: false,
                                rotateX: 600,
                                rotateY: 1200,
                            },
                        },
                    },
                    interactivity: {
                        detect_on: "canvas" as InteractivityDetect,
                        events: {
                            onhover: {
                                // enable: true,
                                // mode: "grab",
                                // parallax: {
                                //     enable: true,
                                //     smooth: 10,
                                //     force: 60,
                                // },
                            },
                            onclick: {
                                enable: true,
                                mode: "push",
                            },
                            resize: true,
                        },
                        modes: {
                            grab: {
                                distance: 400,
                                line_linked: {
                                    opacity: 1,
                                },
                            },
                            bubble: {
                                distance: 400,
                                size: 40,
                                duration: 2,
                                opacity: 0.8,
                                // speed: 3,
                            },
                            repulse: {
                                distance: 200,
                            },
                            push: {
                                particles_nb: 4,
                            },
                            remove: {
                                particles_nb: 2,
                            },
                        },
                    },
                    retina_detect: true,
                    background: {
                        color: "transparent",
                        image: "",
                        position: "50% 50%",
                        repeat: "no-repeat",
                        size: "cover",
                    },
                }}
            />
        );
    }
}
